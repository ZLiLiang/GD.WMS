<template>
    <div class="app-container">
        <el-form :model="queryParams" ref="queryRef" v-show="showSearch" :inline="true">
            <el-form-item label="公司名称" prop="companyName">
                <el-input v-model="queryParams.companyName" placeholder="请输入公司名称" clearable style="width: 163px" />
            </el-form-item>
            <el-form-item label="负责人" prop="manager">
                <el-input v-model="queryParams.manager" placeholder="请输入负责人" clearable style="width: 163px" />
            </el-form-item>
            <el-form-item label="联系方式" prop="contactTel">
                <el-input v-model="queryParams.contactTel" placeholder="请输入联系方式" clearable style="width: 163px" />
            </el-form-item>
            <el-form-item label="创建时间">
                <el-date-picker v-model="dateRange" style="width: 175px" type="daterange" range-separator="-"
                    start-placeholder="开始日期" end-placeholder="结束日期"></el-date-picker>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="search" @click="handleQuery">搜索</el-button>
                <el-button icon="refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <el-row :gutter="10">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="warning" plain icon="download" @click="handleExport">导出</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList" :columns="columns"></right-toolbar>
        </el-row>

        <el-table v-loading="loading" :data="companyList" highlight-current-row>
            <el-table-column label="编号" prop="companyId" align="center" width="80" v-if="columns.showColumn('companyId')" />
            <el-table-column label="公司名称" prop="companyName" align="center" v-if="columns.showColumn('companyName')"
                :show-overflow-tooltip="true" />
            <el-table-column label="所在城市" prop="city" align="center" v-if="columns.showColumn('city')" />
            <el-table-column label="详细地址" prop="address" align="center" v-if="columns.showColumn('address')"
                :show-overflow-tooltip="true" />
            <el-table-column label="负责人" prop="manager" align="center" v-if="columns.showColumn('manager')" />
            <el-table-column label="联系方式" prop="contactTel" align="center" v-if="columns.showColumn('contactTel')" />
            <el-table-column label="创建时间" prop="createTime" align="center" v-if="columns.showColumn('createTime')" />
            <el-table-column label="操作" align="center" width="120" v-if="columns.showColumn('operate')">
                <template #default="scope">
                    <div>
                        <el-button text icon="edit" title="编辑" @click.stop="handleUpdate(scope.row)">
                        </el-button>
                        <el-button text icon="delete" title="删除" @click.stop="handleDelete(scope.row)">
                        </el-button>
                    </div>
                </template>
            </el-table-column>

        </el-table>

        <pagination v-show="total > 0" :total="total" v-model:page="queryParams.pageNum"
            v-model:limit="queryParams.pageSize" @pagination="getList" />

        <!-- 新增公司信息对话框 -->
        <el-dialog :title="title" v-model="open" width="600px" append-to-body>
            <el-form :model="form" :rules="rules" ref="companyRef" label-width="80px">
                <el-form-item label="公司名称" prop="companyName">
                    <el-input v-model="form.companyName" placeholder="请输入公司名称" />
                </el-form-item>
                <el-form-item label="所在城市" prop="city">
                    <el-input v-model="form.city" placeholder="请输入所在城市" />
                </el-form-item>
                <el-form-item label="详细地址" prop="address">
                    <el-input v-model="form.address" placeholder="请输入详细地址" />
                </el-form-item>
                <el-form-item label="负责人" prop="manager">
                    <el-input v-model="form.manager" placeholder="请输入负责人" />
                </el-form-item>
                <el-form-item label="联系方式" prop="contactTel">
                    <el-input v-model="form.contactTel" placeholder="请输入联系方式" maxlength="11" />
                </el-form-item>
            </el-form>

            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>

    </div>
</template>

<script setup>
import { getAll, getCompanyInfo, addCompanyInfo, editCompanyInfo, exportCompany, deleteById } from "@/api/warehousManagement/company";

// 总条数
const total = ref(0)
const showSearch = ref(false)
const loading = ref(true)
const companyList = ref([])
const open = ref(false)
const title = ref('')
const dateRange = ref([])

const data = reactive({
    form: {},
    queryParams: {
        pageNum: 1,
        pageSize: 10,
        companyName: undefined,
        manager: undefined,
        contactTel: undefined
    },
    rules: {
        companyName: [
            {
                required: true,
                message: '公司名称不能为空',
                trigger: 'blur'
            }
        ],
        city: [
            {
                required: true,
                message: '所在城市不能为空',
                trigger: 'blur'
            }
        ],
        address: [
            {
                required: true,
                message: '详细地址不能为空',
                trigger: 'blur'
            }
        ],
        manager: [
            {
                required: true,
                message: '负责人不能为空',
                trigger: 'blur'
            }
        ],
        contactTel: [
            {
                required: true,
                message: '电话格式:固话或13、14、15、17、18开头+9位阿拉伯数字',
                trigger: 'blur',
                pattern: /^((0\d{2,3}-\d{7,8})|(1[34578]\d{9}))$/,
            }
        ]
    }
})

// 列显隐信息
const columns = ref([
    { key: 0, label: `编号`, visible: true, prop: 'companyId' },
    { key: 1, label: `公司名称`, visible: true, prop: 'companyName' },
    { key: 2, label: `所在城市`, visible: true, prop: 'city' },
    { key: 3, label: `详细地址`, visible: true, prop: 'address' },
    { key: 4, label: `负责人`, visible: true, prop: 'manager' },
    { key: 5, label: `联系方式`, visible: true, prop: 'contactTel' },
    { key: 6, label: `创建时间`, visible: false, prop: 'createTime' },
    { key: 7, label: `操作`, visible: true, prop: 'operate' }
])

const { form, queryParams, rules } = toRefs(data)
const { proxy } = getCurrentInstance()

/** 重置操作表单 */
function reset() {
    form.value = {
        companyName: undefined,
        city: undefined,
        address: undefined,
        manager: undefined,
        contactTel: undefined
    }
    proxy.resetForm('companyRef')
}

/** 查询公司列表 */
function getList() {
    loading.value = true
    getAll(proxy.addDateRange(queryParams.value, dateRange.value)).then(res => {
        loading.value = false
        companyList.value = res.data.result
        total.value = res.data.totalNum
    })
}

/** 新增按钮操作 */
function handleAdd() {
    reset()
    open.value = true
    title.value = '添加公司信息'
}

/** 导出按钮操作 */
function handleExport() {
    proxy.$modal
        .confirm('是否确认导出所有公司信息数据项?', '警告', {
            confirmButtonText: '确定',
            cancelButtonText: '取消',
            type: 'warning'
        })
        .then(async () => {
            await exportCompany()
        })
}

/** 提交按钮 */
function submitForm() {
    proxy.$refs['companyRef'].validate((valid) => {
        console.log(valid);
        if (valid) {
            if (form.value.companyId != undefined) {
                editCompanyInfo(form.value.companyId, form.value).then(res => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    getList()
                })
            } else {
                addCompanyInfo(form.value).then(res => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    getList()
                })
            }
        }
    })
}

/** 取消按钮 */
function cancel() {
    open.value = false
    reset()
}

/** 修改按钮操作 ok */
function handleUpdate(row) {
    reset()
    const companyId = row.companyId
    getCompanyInfo(companyId).then((response) => {
        form.value = response.data
        open.value = true
        title.value = '修改角色'
    })
}

/** 删除按钮操作 */
function handleDelete(row) {
    const companyId = row.companyId
    proxy.$modal
        .confirm('是否确认删除用户编号为"' + companyId + '"的数据项？')
        .then(function () {
            return deleteById(companyId)
        })
        .then(() => {
            getList()
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

/** 搜索按钮操作 */
function handleQuery() {
    queryParams.pageNum = 1
    getList()
}

/** 重置按钮操作 */
function resetQuery() {
    dateRange.value = []
    proxy.resetForm('queryRef')
    handleQuery()
}

getList()
</script>

<style></style>
