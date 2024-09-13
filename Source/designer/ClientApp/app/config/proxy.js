
//proxy setup by using express framework
const express = require('express');
const { createProxyMiddleware } = require('http-proxy-middleware');

const app = express();
app.use(
    '/api',
    createProxyMiddleware({
        target: 'http://localhost:6001',
        changeOrigin: true,
        logLevel: 'debug',
        pathRewrite: {
            '^/api': 'http://localhost:6001/api',
        },
    }),
);
app.listen(6000);