using CloudComputingA3.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHostedService<RouteCheckBackgroundService>();

builder.Services.AddSession(options =>
{
    options.Cookie.IsEssential = true;
});
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

DateTime currentDateTime = DateTime.UtcNow;
DateTime date = currentDateTime.Date;
TimeSpan time = currentDateTime.TimeOfDay;


string timeString = time.ToString(@"hh\:mm\:ss");
string dateString = date.ToString("dd/MM/yyyy");

app.Run();
