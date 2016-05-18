#How to build

You need mono `brew install mono`

I'm using VSCode + Ionide

```
npm install -g foreman
```

You will need a postgres database. By default the app looks for its connection string
in the environment variable `DATABASE_URL`.


### Docker Setup

#### OSX

`brew update`
`brew install docker`
`brew install boot2docker` (It's vagrant for Docker containers, uses hosts docker binary)
`boot2docker init`
`boot2docker up` (Follow the trailing instructions for ENV vars OR cert install)
`docker build -f ./Dockerfile -t="cache" .` 

#API

`~/cache/<owner>/<repo>/issues/<issueid>` - gets and caches (forever) a github issue

### Scripts

#### Set Up Database

In `~/migrations/up` are a set of SQL files you can run to construct the database

You will need to run them as admin. they create the user and database.

#### Compile Code

`./build.sh`

#### Run Tests
`./build.sh test`

#### Run App
`./run.sh`

or

`foreman start` - if ruby based

or

`nf start` - if node based

or

`docker run -t -p 5000:5000 cache` (cache is what i named my docker image)
