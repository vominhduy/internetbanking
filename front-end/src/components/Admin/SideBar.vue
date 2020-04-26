<template>
    <div>
        <b-card no-body title="Title" header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <h6 class="mb-0">Categories</h6>
            </template>
            <b-list-group>
                 <b-list-group-item :to="``" >
                    Quản lý danh sách nhân viên
                </b-list-group-item>
                <b-list-group-item :to="``" >
                    Xem danh sách giao dịch trong tháng với các ngân hàng khác
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