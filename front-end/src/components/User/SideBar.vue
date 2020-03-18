<template>
    <div>
        <b-card no-body title="Title" header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <h6 class="mb-0">Categories</h6>
            </template>
            <b-list-group>
                <b-list-group-item :to="`/employee/`" >
                    Danh sách tài khoản
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Thiết lập danh bạ
                </b-list-group-item>
                <b-list-group-item :to="`/user/remittance`" >
                    Chuyển khoản
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Quản lý nhắc nợ
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Xem lịch sử giao dịch
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Đổi mật khẩu
                </b-list-group-item>
            </b-list-group>
        </b-card>
    </div>
</template>

<script>
    //import axios from "axios";
    export default {
        name: 'SideBar',
    }
</script>

<style scoped>

</style>