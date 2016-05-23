if [ "$1" == "docker" ]
then
    docker run -t -p 5000:5000 --env-file ./.env cache
else
  source .env
  mono ./build/hucache.http.exe
fi
