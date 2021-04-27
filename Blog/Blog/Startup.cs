using Blog.Data;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System; //khai báo sử dụng namespace System (chứa lớp Console)
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Twilio;

namespace Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<BlogDbContext>(options => {
                // Đọc chuỗi kết nối
                string connectstring = Configuration.GetConnectionString("AppDbContext");
                // Sử dụng MS SQL Server
                options.UseSqlServer(connectstring);
            });
            services.AddIdentity<User, Role>() // Thêm vào dịch vụ Identity với cấu hình mặc định cho User (model user) vào Role (model Role - vai trò)
            .AddEntityFrameworkStores<BlogDbContext>() //Thêm triển khai EF lưu trữ thông tin về Idetity(theo BlogDbContext -> MS SQL Server).
            .AddDefaultUI()
            .AddDefaultTokenProviders() //Thêm Token Provider - nó sử dụng để phát sinh token
            .AddUserStore<UserStore<User, Role, BlogDbContext, long>>()
            .AddRoleStore<RoleStore<Role, BlogDbContext, long>>()
            .AddErrorDescriber<CustomIdentityErrorDescriber>();
            services.AddControllersWithViews();
            services.Configure<IdentityOptions>(options =>
            {
                //Thiết lập password
                options.Password.RequireDigit = false; //Không bắt buộc có số
                options.Password.RequiredLength = 4; //Độ dài tối thiểu = 4
                options.Password.RequireLowercase = false; //Không bắt buộc có chữ thường
                options.Password.RequireUppercase = false; //Không bắt buộc có chữ hoa
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt
                options.Password.RequireNonAlphanumeric = false; //Không bắt buộc có ký tự đặc biệt

                //Thiết lập lockout
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); //Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 3; //Số lần đăng nhập thất bại max = 3

                //Thiết lập user
                options.User.AllowedUserNameCharacters = //Ký tự cho phép có trong tên user
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true; //Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true; // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            });

            services.AddOptions();                                        // Kích hoạt Options
            var mailsettings = Configuration.GetSection("MailSettings");  // đọc config
            services.Configure<MailSettings>(mailsettings);               // đăng ký để Inject
            services.AddTransient<IEmailSender, SendMailService>();       // Đăng ký dịch vụ Mail

            services.AddTransient<ISMSSender, AuthMessageSender>();
            services.Configure<SMSoptions>(Configuration.GetSection("SMS"));

            services.Configure<RouteOptions>(options =>
            {
                options.AppendTrailingSlash = false;         //thêm dấu / vào cuối url
                options.LowercaseUrls = true;               // url chữ thường
                options.LowercaseQueryStrings = false;     // không bắt query trong url phải in thường
            });

            services.ConfigureApplicationCookie(options => {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                options.AccessDeniedPath = $"/Account/AccessDenied";
            });
            
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanViewProduct", policy => policy.RequireClaim("permission","product.view"));
                options.AddPolicy("CanCreateProduct", policy => policy.RequireClaim("permission", "product.create"));
                options.AddPolicy("CanUpdateProduct", policy => policy.RequireClaim("permission", "product.update"));
                options.AddPolicy("CanDeleteProduct", policy => policy.RequireClaim("permission", "product.delete"));
                options.AddPolicy("CanCreateUser", policy => policy.RequireClaim("permission", "user.create"));
                options.AddPolicy("CanDeleteUser", policy => policy.RequireClaim("permission", "user.delete"));
            });

            services.AddDistributedMemoryCache();           // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
            services.AddSession(cfg => {                    // Đăng ký dịch vụ Session
                cfg.Cookie.Name = "blogmvc";             // Đặt tên Session - tên này sử dụng ở Browser (Cookie)
                cfg.IdleTimeout = new TimeSpan(0, 30, 0);    // Thời gian tồn tại của Session
            });

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    // Đọc thông tin Authentication:Facebook từ appsettings.json
                    IConfigurationSection facebookAuthNSection = Configuration.GetSection("Authentication:Facebook");

                    facebookOptions.AppId = facebookAuthNSection["AppId"];
                    facebookOptions.AppSecret = facebookAuthNSection["AppSecret"];
                })
                .AddGoogle(googleOptions =>
                {
                    // Đọc thông tin Authentication:Google từ appsettings.json
                    IConfigurationSection googleAuthNSection = Configuration.GetSection("Authentication:Google");

                    // Thiết lập ClientID và ClientSecret để truy cập API google
                    googleOptions.ClientId = googleAuthNSection["ClientId"];
                    googleOptions.ClientSecret = googleAuthNSection["ClientSecret"];
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
