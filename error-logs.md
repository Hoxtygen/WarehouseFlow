[16:28:34 INF] Request starting HTTP/1.1 POST http://localhost:5089/api/v1/orders - application/json 237
[16:28:34 INF] Executing endpoint 'WarehouseFlow.Api.Controllers.OrderController.CreateOrder (WarehouseFlow.Api)'
[16:28:34 INF] Route matched with {action = "CreateOrder", controller = "Order"}. Executing controller action with signature System.Threading.Tasks.Task`1[Microsoft.AspNetCore.Mvc.IActionResult] CreateOrder(WarehouseFlow.Application.Dtos.OrderDto, System.Threading.CancellationToken) on controller WarehouseFlow.Api.Controllers.OrderController (WarehouseFlow.Api).
[16:28:35 INF] Executed DbCommand (6ms) [Parameters=[@applicationUserId='?'], CommandType='Text', CommandTimeout='30']
SELECT c."Id", c."ApplicationUserId", c."CreatedAt", c."Email", c."FirstName", c."LastName", c."PhoneNumber", c."UpdatedAt", c."Address_City", c."Address_HouseNumber", c."Address_LandMark", c."Address_State", c."Address_Street"
FROM "Customers" AS c
WHERE c."ApplicationUserId" = @applicationUserId
LIMIT 1
[16:28:35 INF] Executed DbCommand (6ms) [Parameters=[@productId='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
SELECT p."Id", p."Brand", p."CreatedAt", p."Description", p."ImageUrl", p."IsActive", p."ProductCategoryId", p."ProductName", p."SKU", p."UnitPrice", p."UpdatedAt"
FROM products AS p
WHERE p."Id" = @productId
LIMIT 1
[16:28:35 INF] Executed DbCommand (5ms) [Parameters=[p0='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
SELECT * FROM inventories WHERE "ProductId" = @p0 ORDER BY "Id" FOR UPDATE
[16:28:35 INF] Executed DbCommand (11ms) [Parameters=[@p2='?' (DbType = Guid), @p0='?' (DbType = Int32), @p1='?' (DbType = DateTime)], CommandType='Text', CommandTimeout='30']
UPDATE inventories SET "AvailableQuantity" = @p0, "LastReservedAt" = @p1
WHERE "Id" = @p2;
[16:28:35 INF] Reserved 25 units of Product 01a033f8-ce9b-7069-9ff1-f6b4a3da6ea4 from 1 warehouse(s)
[16:28:35 INF] Executed DbCommand (1ms) [Parameters=[@productId='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
SELECT p."Id", p."Brand", p."CreatedAt", p."Description", p."ImageUrl", p."IsActive", p."ProductCategoryId", p."ProductName", p."SKU", p."UnitPrice", p."UpdatedAt"
FROM products AS p
WHERE p."Id" = @productId
LIMIT 1
[16:28:35 INF] Executed DbCommand (1ms) [Parameters=[p0='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
SELECT * FROM inventories WHERE "ProductId" = @p0 ORDER BY "Id" FOR UPDATE
[16:28:35 INF] Executed DbCommand (1ms) [Parameters=[@p2='?' (DbType = Guid), @p0='?' (DbType = Int32), @p1='?' (DbType = DateTime)], CommandType='Text', CommandTimeout='30']
UPDATE inventories SET "AvailableQuantity" = @p0, "LastReservedAt" = @p1
WHERE "Id" = @p2;
[16:28:35 INF] Reserved 10 units of Product 01a03406-e560-7c73-9b0a-b54c03751d1b from 1 warehouse(s)
[16:28:35 INF] Executed DbCommand (17ms) [Parameters=[@p0='?' (DbType = DateTime), @p1='?' (DbType = Guid), @p2='?' (DbType = DateTime), @p3='?', @p4='?' (DbType = Decimal)], CommandType='Text', CommandTimeout='30']
INSERT INTO orders ("CreatedAt", "CustomerId", "OrderDate", "OrderStatus", "TotalAmount")
VALUES (@p0, @p1, @p2, @p3, @p4)
RETURNING "Id", "UpdatedAt";
[16:28:35 INF] Executed DbCommand (18ms) [Parameters=[@p5='?' (DbType = DateTime), @p6='?' (DbType = Guid), @p7='?' (DbType = Guid), @p8='?' (DbType = Int32), @p9='?' (DbType = Decimal), @p10='?' (DbType = DateTime), @p11='?' (DbType = Guid), @p12='?' (DbType = Guid), @p13='?' (DbType = Int32), @p14='?' (DbType = Decimal), @p15='?' (DbType = DateTime), @p16='?' (DbType = DateTime), @p17='?' (DbType = Guid), @p18='?' (DbType = Guid), @p19='?' (DbType = Int32), @p20='?' (DbType = Guid), @p21='?' (DbType = DateTime), @p22='?' (DbType = DateTime), @p23='?' (DbType = Guid), @p24='?' (DbType = Guid), @p25='?' (DbType = Int32), @p26='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
INSERT INTO order_items ("CreatedAt", "OrderId", "ProductId", "Quantity", "UnitPrice")
VALUES (@p5, @p6, @p7, @p8, @p9)
RETURNING "Id", "UpdatedAt";
INSERT INTO order_items ("CreatedAt", "OrderId", "ProductId", "Quantity", "UnitPrice")
VALUES (@p10, @p11, @p12, @p13, @p14)
RETURNING "Id", "UpdatedAt";
INSERT INTO reservations ("CreatedAt", "ExpiresAt", "OrderId", "ProductId", "ReservedQuantity", "WarehouseId")
VALUES (@p15, @p16, @p17, @p18, @p19, @p20)
RETURNING "Id", "UpdatedAt";
INSERT INTO reservations ("CreatedAt", "ExpiresAt", "OrderId", "ProductId", "ReservedQuantity", "WarehouseId")
VALUES (@p21, @p22, @p23, @p24, @p25, @p26)
RETURNING "Id", "UpdatedAt";
[16:28:35 INF] Order created with ID 01a043d6-57d9-7af0-a7f9-e4c2fc1afc4f for Customer 01a02586-0f98-7738-98a1-86467726413e with total amount 4749.65
[16:28:35 INF] Executing CreatedAtActionResult, writing value of type 'WarehouseFlow.Api.Contracts.ApiResponse`1[[WarehouseFlow.Domain.Entities.Order, WarehouseFlow.Domain, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null]]'.
[16:28:36 INF] Executed action WarehouseFlow.Api.Controllers.OrderController.CreateOrder (WarehouseFlow.Api) in 1050.9599ms
[16:28:36 INF] Executed endpoint 'WarehouseFlow.Api.Controllers.OrderController.CreateOrder (WarehouseFlow.Api)'
[16:28:36 ERR] An error occurred: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles. Path: $.Data.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.CustomerId.
System.Text.Json.JsonException: A possible object cycle was detected. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32. Consider using ReferenceHandler.Preserve on JsonSerializerOptions to support cycles. Path: $.Data.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.OrderItems.Order.CustomerId.
   at System.Text.Json.ThrowHelper.ThrowJsonException_SerializerCycleDetected(Int32 maxDepth)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Converters.IEnumerableDefaultConverter`2.OnWriteResume(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryWrite(Utf8JsonWriter writer, TCollection value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.GetMemberAndWriteJson(Object obj, WriteStack& state, Utf8JsonWriter writer)
   at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryWrite(Utf8JsonWriter writer, T value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.TryWrite(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.JsonConverter`1.WriteCore(Utf8JsonWriter writer, T& value, JsonSerializerOptions options, WriteStack& state)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
   at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.SerializeAsync(PipeWriter pipeWriter, T rootValue, Int32 flushThreshold, CancellationToken cancellationToken, Object rootValueBoxed)
   at Microsoft.AspNetCore.Mvc.Formatters.SystemTextJsonOutputFormatter.WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeNextResultFilterAsync>g__Awaited|30_0[TFilter,TFilterAsync](ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.Rethrow(ResultExecutedContextSealed context)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.ResultNext[TFilter,TFilterAsync](State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.InvokeResultFilters()
--- End of stack trace from previous location ---
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Logged|17_1(ResourceInvoker invoker)
   at Microsoft.AspNetCore.Routing.EndpointMiddleware.<Invoke>g__AwaitRequestTask|7_0(Endpoint endpoint, Task requestTask, ILogger logger)
   at Microsoft.AspNetCore.Authorization.AuthorizationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Swashbuckle.AspNetCore.SwaggerUI.SwaggerUIMiddleware.Invoke(HttpContext httpContext)
   at Swashbuckle.AspNetCore.Swagger.SwaggerMiddleware.Invoke(HttpContext httpContext, ISwaggerProvider swaggerProvider)
   at WarehouseFlow.Api.Middleware.ExceptionHandlingMiddleware.InvokeAsync(HttpContext context) in C:\Users\ADMIN\Documents\CSharp\Truth\WarehouseFlow\src\WarehouseFlow.Api\Middleware\ExceptionHandlingMiddleware.cs:line 33