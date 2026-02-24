var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
  app.MapGet("/chisme", () => {
    try {
        var root = System.IO.Directory.GetCurrentDirectory();
        var files = System.IO.Directory.GetFiles(root, "*", System.IO.SearchOption.AllDirectories);
        return "ARCHIVOS EN EL SERVIDOR:\n\n" + string.Join("\n", files);
    } catch (System.Exception ex) {
        return "Error leyendo archivos: " + ex.Message;
    }
});
app.Run();
