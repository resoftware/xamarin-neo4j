import json, os
from googleapiclient.discovery import build
from google.oauth2 import service_account

creds = service_account.Credentials.from_service_account_info(
    json.loads(os.environ['PLAY_SERVICE_ACCOUNT_JSON']),
    scopes=['https://www.googleapis.com/auth/androidpublisher']
)
service = build('androidpublisher', 'v3', credentials=creds)
edit = service.edits().insert(packageName='nl.resoftware.pocketgraph', body={}).execute()
bundles = service.edits().bundles().list(
    packageName='nl.resoftware.pocketgraph',
    editId=edit['id']
).execute()
service.edits().delete(packageName='nl.resoftware.pocketgraph', editId=edit['id']).execute()
max_code = max([b['versionCode'] for b in bundles.get('bundles', [])], default=0)
print(max_code + 1)
