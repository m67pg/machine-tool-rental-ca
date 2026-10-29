# ---- ビルド ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# csprojだけ先にコピーして restore（変更が少ない層をキャッシュさせる）
COPY machine-tool-rental-ca/machine-tool-rental-ca.csproj machine-tool-rental-ca/
RUN dotnet restore machine-tool-rental-ca/machine-tool-rental-ca.csproj

COPY machine-tool-rental-ca/ machine-tool-rental-ca/

# Windows側（Visual Studio）で作られた obj/bin が紛れ込んでも使わないよう、念のため削除してから publish する
# （.dockerignore が効いていれば通常は存在しない）
RUN rm -rf machine-tool-rental-ca/obj machine-tool-rental-ca/bin \
    && dotnet publish machine-tool-rental-ca/machine-tool-rental-ca.csproj \
       -c Release -o /app/publish

# ---- 実行 ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# ghcr.io のパッケージをこのリポジトリに紐づける（GitHub Actionsから継続して登録できるようにする）
LABEL org.opencontainers.image.source="https://github.com/m67pg/machine-tool-rental-ca"

ENV ASPNETCORE_ENVIRONMENT=Production
# Container Appsのイングレス（HTTPS終端）の背後で、元のHTTPS/クライアント情報を正しく扱う
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

# 公式イメージは既定で 8080 を待ち受ける（Container Appsのtarget portも8080にする）
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "machine-tool-rental-ca.dll"]
