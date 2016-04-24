#How to build

You need mono `brew install mono`

I'm using VSCode + Ionide

You will need a postgres database.

#API

`~/cache/<owner>/<repo>/issues/<issueid>` - gets and caches (forever) a github issue

### Scripts

#### Compile Code

`./build.sh`

#### Run Tests
`./build.sh test`

#### Run App
`./run.sh`