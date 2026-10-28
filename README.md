# machine-tool-rental-ca

ASP.NET Core + Blazor（.NET 10）/ Neon（PostgreSQL）/ Azure Container Apps のデモアプリ。

## ローカル起動

1. `machine-tool-rental-ca.sln` をVisual Studioで開く
2. プロジェクトのフォルダで接続文字列をユーザーシークレットに登録

       cd machine-tool-rental-ca
       dotnet user-secrets set "ConnectionStrings:Default" "Host=（Neonのホスト）;Port=5432;Database=neondb;Username=（ユーザー）;Password=（パスワード）;SSL Mode=Require"

3. マイグレーションを作成（初回のみ）

       dotnet tool install --global dotnet-ef
       dotnet ef migrations add InitialCreate

4. F5 で起動（起動時にテーブル作成と固定データ投入が行われる）

## 管理画面

- `/admin/machines`
- ID `admin` / パスワード `demo-password`（`Data/DbSeeder.cs` の定数）
