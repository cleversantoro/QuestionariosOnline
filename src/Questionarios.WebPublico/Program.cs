using Microsoft.Extensions.Options;
using Questionarios.WebPublico.Models;
using Questionarios.WebPublico.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("Api"));

builder.Services.AddHttpClient<ISurveyApiClient, SurveyApiClient>((sp, client) =>
{
    var apiSettings = sp.GetRequiredService<IOptions<ApiSettings>>().Value;
    if (string.IsNullOrWhiteSpace(apiSettings.BaseUrl))
    {
        throw new InvalidOperationException("Api:BaseUrl não configurado.");
    }

    client.BaseAddress = new Uri(apiSettings.BaseUrl);
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Survey}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
