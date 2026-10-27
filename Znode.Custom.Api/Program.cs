using Znode.Engine.Services.Inventory;
using Znode.Engine.Services.Payment;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowStorefront", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddScoped<INetSuiteInventoryService, NetSuiteInventoryService>();
builder.Services.AddScoped<IPaymentGatewayService, PaymentGatewayService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowStorefront");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();