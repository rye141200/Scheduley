export const environment = {
  production: false,
  serverUrl: 'http://localhost:3000',
  apiURL: 'https://localhost:7116/',
  googleLoginUrlString: `https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id=667150783706-qcgrp3hca7hn5jfmehurlpbts84skp0h.apps.googleusercontent.com&redirect_uri=https://localhost:7116/api/account/google-callback-angular&scope=openid email profile&state=${crypto.randomUUID()}`,
};
//
