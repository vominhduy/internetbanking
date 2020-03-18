<template>
    <div>
        <b-card no-body title="Title" header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <h6 class="mb-0">Categories</h6>
            </template>
            <b-list-group>
                <b-list-group-item :to="`/employee/create-user`" >
                    Tạo tài khoản khách hàng
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Nạp tiền tài khoản khách hàng
                </b-list-group-item>
                <b-list-group-item :to="`/employee/`" >
                    Xem lịch sử giao dịch
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