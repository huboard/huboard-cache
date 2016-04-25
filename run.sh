if [ -e ".env" ]
then
    #do nothing
else
    #load default connection string
    echo "PG=User ID=hucache;Password=;Host=localhost;Port=5432;Database=hucache;
Pooling=false;" > .env
fi

source .env
mono ./build/hucache.http.exe