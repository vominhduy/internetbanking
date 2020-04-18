var axios = require("axios");
var md5 = require('md5');

module.exports = {
    // Thời gian sử dụng token lần gần nhất
    last_usded: 0,
    base_url: 'https://localhost:5001/api/',
    secret_key: '75836f6ded2047c4b1f5770c3229fc02',
    token_time: 100,

    auto_get_access_token() {
        var access_token_new = ""
        var last_time = new Date((new Date(this.last_usded)).toUTCString() + this.token_time * 60000);
        if ((Math.round(last_time.getTime() / 1000)) > Math.round((new Date()).getTime() / 1000)) {
            // lấy access_token mới và lưu lại
            // TODO
            access_token_new = localStorage.getItem('access_token')
        } else {
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
        } else {
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
            // TODO
            console.log(this.last_usded)
        } else if (method === "put") {
            // TODO
            console.log(this.last_usded)
        } else if (method === "post") {
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