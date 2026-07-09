namespace HostelEase.UI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static WebApplication UseApplicationPipeline(this WebApplication app) 
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error"); // This stays the same
            }

            // Change this to use the controller route with status code parameter
            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapControllers();
            
            return app;
        }
    }
}
