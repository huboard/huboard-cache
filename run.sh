if [ -e ".env" ]
then
    #do nothing
    echo "hi"
else
    #load default connection string
    echo "DATABASE_URL=postgres://hucache:@localhost:5432/hucache" > .env
fi

source .env
mono ./build/hucache.http.exe