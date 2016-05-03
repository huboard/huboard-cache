if [ -e ".env" ]
then
    #do nothing
    echo "hi"
else
    #load default connection string
    echo "DATABASE_URL=postgres://hucache:hucache@localhost:5432/hucache" > .env
fi

if [ $0 == "docker" ]
then
    docker run -t -p 5000:5000 --env-file ./.env cache
else
  source .env
  mono ./build/hucache.http.exe
fi
