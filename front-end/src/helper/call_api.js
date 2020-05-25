var axios = require("axios");
var md5 = require('md5');

module.exports = {
    // Thời gian sử dụng token lần gần nhất
    last_usded: 0,
    base_url: 'http://www.ddpbank.somee.com/api/',
    //base_url: 'https://localhost:44396/api/',
    secret_key: '75836f6ded2047c4b1f5770c3229fc02',
    token_time: 100,

    auto_get_access_token() {
        var access_token_new = ""
        var last_time = new Date((new Date(this.last_usded)).toUTCString() + this.token_time * 60000);
        if ((Math.round(last_time.getTime() / 1000)) > Math.round((new Date()).getTime() / 1000)) {
            // lấy access_token mới và lưu lại
            var obj = {
                AccessToken: localStorage.getItem('access_token'),
                RefreshToken: "TODO: lấy refresh token lưu ở đâu t k biết"
            };
            var timestamp = Math.round((new Date()).getTime() / 1000);
            var hash = md5(JSON.stringify(obj) + this.secret_key + timestamp)

            const refreshToken = axios.create({
                baseURL: this.base_url,
                timeout: 100000,
                headers: {
                    'partner_code': this.secret_key,
                    'timestamp': timestamp,
                    'hash': hash,
                }
            });
            refreshToken.post('Accounts/Tokens/Refresh', obj)
                .then(res => {
                    access_token_new = res.data.AccessToken
                    // TODO lưu lại accesstoken và refresh token
                })
                .catch(err => {
                    console.log(err);
                });
        }
        else {
            // dùng access_token hiện tại
            access_token_new = localStorage.getItem('access_token')
        }
        return access_token_new;
    },

    call_api(url, method, request) {
        var timestamp = Math.round((new Date()).getTime() / 1000);
        var hash = "";
        if (method === "get") {
            hash = md5("" + this.secret_key + timestamp)
        }
        else {
            hash = md5(JSON.stringify(request) + this.secret_key + timestamp)
        }

        // add header
        const instance = axios.create({
            baseURL: this.base_url,
            timeout: 100000,
            headers: {
                'partner_code': this.secret_key,
                'timestamp': timestamp,
                'hash': hash,
                'authorization': 'Bearer ' + this.auto_get_access_token()
            }
        });

        if (method === "delete") {
            this.last_usded = Math.round((new Date()).getTime() / 1000);
            console.log(this.last_usded)
            return new Promise((resolve, reject) => {
                instance.delete(url)
                    .then(res => {
                        resolve(res);
                    })
                    .catch(err => {
                        reject(err)
                    });
            });
        } else if (method === "put") {
            this.last_usded = Math.round((new Date()).getTime() / 1000);
            return new Promise((resolve, reject) => {
                instance.put(url, request)
                    .then(res => {
                        resolve(res);
                    })
                    .catch(err => {
                        reject(err)
                    });
            });
        }
        else if (method === "post") {
            this.last_usded = Math.round((new Date()).getTime() / 1000);
            return new Promise((resolve, reject) => {
                instance.post(url, request)
                    .then(res => {
                        resolve(res);
                    })
                    .catch(err => {
                        reject(err)
                    });
            });
        } else {
            this.last_usded = Math.round((new Date()).getTime() / 1000);
            return new Promise((resolve, reject) => {
                instance.get(url)
                    .then(res => {
                        resolve(res);
                    })
                    .catch(err => {
                        reject(err)
                    });
            });
        }
    }
}