<template>
    <div>
        <b-card no-body title="Title" header-tag="header" footer-tag="footer">
            <template v-slot:header>
                <h6 class="mb-0">Categories</h6>
            </template>
            <b-list-group>
                <b-list-group-item v-for="item in list.data" :key="item.id" :to="`/employee/${item.id}`"
                >
                    {{item.employee_name}}
                </b-list-group-item>
            </b-list-group>
        </b-card>
    </div>
</template>

<script>
    import axios from "axios";
    export default {
        name: 'Categories',
        
        data() {
            return {
                list: []
            }
        },

        mounted() {
            axios
                .get('http://dummy.restapiexample.com/api/v1/employees')
                .then(res => {
                    this.list = res.data
                })
                .catch(err => {
                    console.error(err);
                })
        }
    }
</script>

<style scoped>

</style>