<template>

    <div>
        
        <b-card no-body title="Title" header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <h6 class="mb-0">Categories</h6>
            </template>
            <b-list-group>
                <b-list-group-item :to="`/admin/ListEmployee`" >
                    Quản lý danh sách nhân viên
                </b-list-group-item>
                 <b-list-group-item :to="`/admin/addStaff`" >
                    Thêm nhân viên
                </b-list-group-item>
                <b-list-group-item :to="`/admin/DoisoatIN`" >
                    Đối soát nhận
                </b-list-group-item>
                <b-list-group-item :to="`/admin/DoisoatOUT`" >
                    Đối soát chuyển
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