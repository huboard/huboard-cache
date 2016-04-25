CREATE TABLE github.issues
(
  owner text NOT NULL,
  repo text NOT NULL,
  issue int NOT NULL,
  headers hstore NOT NULL,
  payload jsonb NOT NULL,
  CONSTRAINT pk_github_issues PRIMARY KEY (owner, repo, issue)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE github.issues
  OWNER TO hucache;