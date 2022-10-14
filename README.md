# google-auth-dotnet
Create events, meeting room with google apis .net 6.0

```
// Microsoft tutorial - for dotnet core 6.0 web
// https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/google-logins?view=aspnetcore-6.0
//  step 1: Install package: dotnet add package Microsoft.AspNetCore.Authentication.Google --version 6.0.10

//  Google Authentication: https://cloud.google.com/looker/docs/admin-panel-authentication-google

// Google dotnet API list
// https://developers.google.com/api-client-library/dotnet
// https://developers.google.com/api-client-library/dotnet/apis
// https://developers.google.com/api-client-library/dotnet/apis/calendar/v3

// Google dotnet API list - calendar (*)
// https://developers.google.com/calendar/api/v3/reference/events/insert#.net
// Nuget - Google dotnet API list - calendar - install
// https://www.nuget.org/packages/Google.Apis.Calendar.v3

// Example: Just with console application 
// https://github.com/googleworkspace/dotnet-samples/blob/main/calendar/CalendarQuickstart/CalendarQuickstart.cs

// Add convert token that from user and change to access (*)
// https://github.com/googleapis/google-api-dotnet-client/issues/1486
// https://stackoverflow.com/questions/38390197/how-to-create-a-instance-of-usercredential-if-i-already-have-the-value-of-access

//  React package
// https://www.npmjs.com/package/@react-oauth/google

// Google user 3rd permission
// Use to remove permissions of applications
// https://myaccount.google.com/permissions

// OAuth2 Protocol Google
// Theory of oauth2
// https://developers.google.com/identity/protocols/oauth2#libraries
```

Event

- Get all
  
  1. GetServiceInstance
    
    - Tải token của người dùng thông qua Id sau khi đã gửi code đăng nhập
      
    - Tạo `user credential` thông qua `flow`, `id`, và token mới tạo
      
    - Trả về service của người dùng tương ứng, service này giup` tạo event, tạo room meet
      
  2. Gọi `service.Events.List` và tên của calendar để lấy danh sách các event của calendar đó
    
  3. Gọi thực thi và trả về
    
- Get By Id
  
  1. Giống get all nhưng sử dụng `service.Events.Get` với tên calendar và id
    
  2. Gọi thực thi và trả về
    
- Create event
  
  1. Kiểm tra id người dùng
    
  2. Tạo `GetServiceInstance`
    
  3. Convert dữ liệu trước khi đưa vào `newEvent`
    
  4. Thực thi `service.events.insert` để tạo một event mới
    
  5. Patch event mới với `conference data` để tạo room meet
    
  6. Thực thi patch để tạo event mới
    

GoogleAuth

1. Nhận `code` từ phía client gửi lên server.
  
2. Server sử dụng API để giải mã code đó thành token của người dùng
  
3. Cấu hình xác thực server và google-application thông qua oAuth2 Key (được lưu trọng `credentials.json`)
  
4. Tạo một đường dẫn đến nơi lưu trữ của token tạm thời của flow `token.json`
  
5. Tạo `GoogleAuthorizationCodeFlow` bằng **secret** từ `credentials.json`, **scopes**, **data store** `token.json`
  
6. Lấy token của người dùng thông qua `id` (có thể thêm `refresh token`) (chỉ với id không thì không chắc phải là của người dùng đó không)
  
7. Sau đó ghi vào `persistent database`
  
8. Trả về kết quả cho người dùng
