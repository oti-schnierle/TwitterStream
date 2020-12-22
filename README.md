# TwitterStub

Application that captures a sample stream using Twitter Api and shows some stats from the data prcocesed


[Twitter Api Sampled stream](https://developer.twitter.com/en/docs/twitter-api/tweets/sampled-stream/api-reference/get-tweets-sample-stream)

### Getting started

1.- Update settings with the Twitter Application Bearer token
```
  "TwitterStubSettings": {
    "Uri": "https://api.twitter.com/2/tweets/sample/stream",
    "Token": "### Update Twitter Token ###"
```

2.- You can change the connections strings to the data: 

-TwitterStubDb for Sql server

-TwitterStubSqlite for Sqlite
```json
"ConnectionStrings": {
    "TwitterStubDb": "Server=(localdb)\\mssqllocaldb;Database=TwitterStubDb;Trusted_Connection=True;MultipleActiveResultSets=true",
    "TwitterStubSqlite": "C:\\Temp\\tweet.db"
  },
```

3.- Configure the location from you want to read emoji.json file:
```
  },
  "MetricsSettings": {
    "EmojiFile": "C:\\Temp\\emoji.json",
    "PhotoDomains": "pic.twitter.com,instagram"
  }
```
4.- Run TwitterStub.Web App

