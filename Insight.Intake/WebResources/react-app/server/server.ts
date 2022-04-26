import express from 'express';
import { createProxyMiddleware, responseInterceptor} from 'http-proxy-middleware';
import fetch from 'node-fetch';
import {RequestInit} from 'node-fetch';
import FormData from 'form-data';
import * as proxyData from '../proxy.json';
import * as packageJson from '../package.json';

(async () => {
  const app = express();
  const getToken = async ():Promise<string | null> =>
  {
    let formData = new FormData();
    formData.append('client_id', proxyData.clientId);
    formData.append('client_secret', proxyData.secret);
    formData.append('resource', proxyData.target);
    formData.append('grant_type', proxyData.grant_type);

    const request:RequestInit =
    {
      method:"POST",
      body: formData
    };

    return (await (await fetch(proxyData.tokenAPI,request)).json())?.access_token;
  }
let token = await getToken();
if(token == null)
{
  console.error(`Error while trying to get access_token. Please check Client Id and secret in the proxy.json file!`);
  return;
}
else
{
  let authHeader = `Bearer ${token}`;

  const proxyurlport = packageJson.proxy.split(':')
  const PORT = Number(proxyurlport[2]);
  const HOST =  proxyurlport[1].replace("//",'');
  
  app.all('*', createProxyMiddleware(
    { target: proxyData.target, 
      changeOrigin: true,
      selfHandleResponse:true,
      onProxyReq:(proxyReq, req, res) => {
        proxyReq.setHeader('Authorization', authHeader);
      },
      onProxyRes: responseInterceptor(async (responseBuffer, proxyRes, req, res) => {
        if(proxyRes.statusCode === 401)
        {
          console.log(`Token is expired`);
          authHeader = `Bearer ${await getToken()}`;
          console.log('Token has been refreshed');
          proxyRes.pipe(res);
          
          return responseBuffer;
        }
        else 
        {
          return responseBuffer;
        }
      })
    }));

  app.listen(PORT, HOST, () =>
    {
      console.log(`Express server is running on ${HOST}:${PORT}`);
    }
  );
}
})();