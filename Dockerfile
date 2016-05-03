FROM fsharp/fsharp
COPY build /app
EXPOSE 8080
ENTRYPOINT ["mono", "/app/hucache.http.exe"]
