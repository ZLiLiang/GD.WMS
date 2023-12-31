<template>
    <div class="app-container">
        <el-form :model="queryParams" ref="queryRef" :inline="true" v-show="showSearch">
            <el-form-item label="上级菜单" prop="parentId">
                <el-cascader class="w100" :options="menuQueryOptions" style="width: 200px"
                    :props="{ checkStrictly: true, value: 'menuId', label: 'menuName', emitPath: false }"
                    placeholder="请选择上级菜单" clearable v-model="queryParams.parentId">
                    <template #default="{ node, data }">
                        <span>{{ data.menuName }}</span>
                        <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
                    </template>
                </el-cascader>
            </el-form-item>
            <el-form-item label="菜单名称" prop="menuName">
                <el-input v-model="queryParams.menuName" placeholder="请输入菜单名称" clearable style="width: 200px" @keyup.enter="handleQuery" />
            </el-form-item>
            <el-form-item label="菜单状态" prop="status">
                <el-select v-model="queryParams.status" placeholder="菜单状态" clearable style="width: 200px">
                    <el-option v-for="dict in sys_normal_disable" :key="dict.Value" :label="dict.Label"
                        :value="dict.Value" />
                </el-select>
            </el-form-item>
            <el-form-item label="是否显示" prop="visible">
                <el-select v-model="queryParams.visible" placeholder="显示状态" clearable style="width: 200px">
                    <el-option v-for="dict in sys_show_hide" :key="dict.Value" :label="dict.Label" :value="dict.Value" />
                </el-select>
            </el-form-item>
            <el-form-item>
                <el-button type="primary" icon="Search" @click="handleQuery">搜索</el-button>
                <el-button icon="Refresh" @click="resetQuery">重置</el-button>
            </el-form-item>
        </el-form>

        <el-row :gutter="10" class="mb8">
            <el-col :span="1.5">
                <el-button type="primary" plain icon="Plus" @click="handleAdd">新增</el-button>
            </el-col>
            <el-col :span="1.5">
                <el-button type="info" plain icon="Sort" @click="toggleExpandAll">展开/折叠</el-button>
            </el-col>
            <right-toolbar v-model:showSearch="showSearch" @queryTable="getList"></right-toolbar>
        </el-row>

        <vxe-table :height="tableHeight" show-overflow ref="listRef" :loading="loading" :column-config="{ resizable: true }"
            :tree-config="{}" :scroll-y="{ enabled: true, gt: 20 }" :data="menuList">
            <vxe-column field="menuName" title="菜单名称" tree-node width="160"> </vxe-column>
            <vxe-column field="menuId" title="菜单id" width="90"></vxe-column>
            <vxe-column field="icon" title="图标" align="center" width="60">
                <template #default="{ row }">
                    <svg-icon :name="row.icon"></svg-icon>
                </template>
            </vxe-column>
            <vxe-column field="menuType" title="菜单类型" align="center" width="80">
                <template #default="scope">
                    <el-tag :disable-transitions="true" type="danger"
                        v-if="scope.row.menuType == 'M' && scope.row.isFrame == 1">链接</el-tag>
                    <el-tag :disable-transitions="true" v-else-if="scope.row.menuType == 'C'">菜单</el-tag>
                    <el-tag :disable-transitions="true" type="success" v-else-if="scope.row.menuType == 'M'">目录</el-tag>
                </template>
            </vxe-column>
            <vxe-column field="orderNum" title="排序" width="90" sortable align="center">
                <template #default="scope">
                    <span v-show="editIndex != scope.row.menuId" @click="editCurrRow(scope.row.menuId)">{{
                        scope.row.orderNum }}</span>
                    <el-input :ref="setColumnsRef" v-show="editIndex == scope.row.menuId" v-model="scope.row.orderNum"
                        @blur="handleChangeSort(scope.row)"></el-input>
                </template>
            </vxe-column>
            <vxe-column field="component" title="组件路径" show-overflow></vxe-column>
            <vxe-column field="visible" title="是否显示" width="90" align="center">
                <template #default="scope">
                    <el-tag :disable-transitions="true" type="success" v-if="scope.row.visible === '0'">可见</el-tag>
                    <el-tag :disable-transitions="true" type="danger" v-else>隐藏</el-tag>
                </template>
            </vxe-column>
            <vxe-column field="status" title="菜单状态" width="80" align="center">
                <template #default="scope">
                    <el-tag :disable-transitions="true" type="success" v-if="scope.row.status === '0'">启用</el-tag>
                    <el-tag :disable-transitions="true" type="danger" v-else>禁用</el-tag>
                </template>
            </vxe-column>
            <vxe-column title="添加时间" align="center" field="createTime" show-overflow>
                <template #default="scope">
                    <span>{{ parseTime(scope.row.createTime) }}</span>
                </template>
            </vxe-column>
            <vxe-column title="操作" align="center" width="140">
                <template #default="scope">
                    <el-button-group>
                        <el-button text size="small" icon="Edit" @click="handleUpdate(scope.row)"></el-button>
                        <el-button text size="small" icon="Plus" @click="handleAdd(scope.row)"></el-button>
                        <el-button text size="small" icon="Delete" @click="handleDelete(scope.row)"></el-button>
                    </el-button-group>
                </template>
            </vxe-column>
        </vxe-table>

        <el-dialog :title="title" v-model="open" width="720px" append-to-body>
            <el-form ref="menuRef" :model="form" :rules="rules" label-width="100px">
                <el-row>
                    <el-col :lg="24">
                        <el-form-item label="上级菜单">
                            <el-cascader class="w100" :options="menuOptions"
                                :props="{ checkStrictly: true, value: 'menuId', label: 'menuName', emitPath: false }"
                                placeholder="请选择上级菜单" clearable v-model="form.parentId">
                                <template #default="{ node, data }">
                                    <span>{{ data.menuName }}</span>
                                    <span v-if="!node.isLeaf"> ({{ data.children.length }}) </span>
                                </template>
                            </el-cascader>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="24">
                        <el-form-item label="菜单类型" prop="menuType">
                            <el-radio-group v-model="form.menuType">
                                <el-radio label="M">目录</el-radio>
                                <el-radio label="C">菜单</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12">
                        <el-form-item label="菜单名称" prop="menuName">
                            <el-input v-model="form.menuName" placeholder="请输入菜单名称" />
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12">
                        <el-form-item label="排序" prop="orderNum">
                            <el-input-number v-model="form.orderNum" controls-position="right" :min="0" />
                        </el-form-item>
                    </el-col>
                    <el-col :lg="24" v-if="form.menuType != 'F'">
                        <el-form-item label="图标" prop="icon">
                            <el-popover placement="bottom-start" :width="540" trigger="click">
                                <template #reference>
                                    <el-input v-model="form.icon" placeholder="点击选择图标" readonly>
                                        <template #prefix>
                                            <svg-icon v-if="form.icon" :name="form.icon" />
                                            <el-icon v-else>
                                                <search />
                                            </el-icon>
                                        </template>
                                    </el-input>
                                </template>
                                <icon-select ref="iconSelectRef" @selected="selected" />
                            </el-popover>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType != 'F'">
                        <el-form-item>
                            <template #label>
                                <span>
                                    <el-tooltip content="选择是外链则路由地址需要以`http(s)://`开头" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    是否外链
                                </span>
                            </template>
                            <el-radio-group v-model="form.isFrame">
                                <el-radio label="1">是</el-radio>
                                <el-radio label="0">否</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType != 'F'">
                        <el-form-item prop="path">
                            <template #label>
                                <span>
                                    <el-tooltip content="访问的路由地址，如：`user`，如外网地址需内链访问则以`http(s)://`开头" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    路由地址
                                </span>
                            </template>
                            <el-input v-model="form.path" placeholder="请输入路由地址" />
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType == 'C'">
                        <el-form-item prop="component">
                            <template #label>
                                <span>
                                    <el-tooltip content="访问的组件路径，如：`system/user/index`，默认在`views`目录下" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    组件路径
                                </span>
                            </template>
                            <el-input v-model="form.component" placeholder="请输入组件路径">
                                <template #prepend>
                                    <span style="width: 40px">src/views/</span>
                                </template>
                            </el-input>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType == 'C'">
                        <el-form-item prop="isCache">
                            <template #label>
                                <span>
                                    <el-tooltip content="选择是则会被`keep-alive`缓存，需要匹配组件的`name`和地址保持一致" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    是否缓存
                                </span>
                            </template>
                            <el-radio-group v-model="form.isCache">
                                <el-radio label="0">是</el-radio>
                                <el-radio label="1">否</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType != 'F'">
                        <el-form-item prop="visible">
                            <template #label>
                                <span>
                                    <el-tooltip content="选择隐藏则路由将不会出现在侧边栏，但仍然可以访问" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    是否显示
                                </span>
                            </template>
                            <el-radio-group v-model="form.visible">
                                <el-radio v-for="dict in sys_show_hide" :key="dict.Value" :label="dict.Value">{{
                                    dict.dictLabel }}</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                    <el-col :lg="12" v-if="form.menuType != 'F'">
                        <el-form-item>
                            <template #label>
                                <span>
                                    <el-tooltip content="选择停用则路由将不会出现在侧边栏，也不能被访问" placement="top">
                                        <el-icon :size="15">
                                            <questionFilled />
                                        </el-icon>
                                    </el-tooltip>
                                    菜单状态
                                </span>
                            </template>
                            <el-radio-group v-model="form.status">
                                <el-radio v-for="dict in sys_normal_disable" :key="dict.Value" :label="dict.Value">{{
                                    dict.Label }}</el-radio>
                            </el-radio-group>
                        </el-form-item>
                    </el-col>
                </el-row>
            </el-form>
            <template #footer>
                <el-button text @click="cancel">取消</el-button>
                <el-button type="primary" @click="submitForm">提交</el-button>
            </template>
        </el-dialog>
    </div>
</template>
  
<script setup name="sysmenu">
import { addMenu, delMenu, getMenu, listMenu, updateMenu, changeMenuSort as changeSort } from '@/api/system/menu'
import SvgIcon from '@/components/SvgIcon'
import IconSelect from '@/components/IconSelect'
import { parseTime } from '@/utils/ruoyi'
const { proxy } = getCurrentInstance()

const menuList = ref([])
const open = ref(false)
const loading = ref(true)
const showSearch = ref(false)
const title = ref('')
const menuOptions = ref([])
const menuQueryOptions = ref([])
const isExpandAll = ref(false)
const iconSelectRef = ref(null)
const menuRef = ref(null)
const listRef = ref(null)
const state = reactive({
    form: {},
    queryParams: {
        menuName: undefined,
        visible: undefined,
        menuTypeIds: 'M,C',
        parentId: undefined
    },
    rules: {
        menuName: [{ required: true, message: '菜单名称不能为空', trigger: 'blur' }],
        menuNameKey: [{ pattern: /^[A-Za-z].+$/, message: '输入格式不正确', trigger: 'blur' }],
        orderNum: [{ required: true, message: '菜单顺序不能为空', trigger: 'blur' }],
        path: [
            { required: true, message: '路由地址不能为空', trigger: 'blur' },
            { pattern: /^[A-Za-z].+$/, message: '输入格式不正确，字母开头', trigger: 'blur' }
        ],
        visible: [{ required: true, message: '显示状态不能为空', trigger: 'blur' }]
    },
    sys_show_hide: [{ Value: "0", Label: "显示" }, { Value: "1", Label: "隐藏" }],
    sys_normal_disable: [{ Value: "0", Label: "正常" }, { Value: "1", Label: "停用" }]
})

const tableHeight = ref(document.documentElement.scrollHeight - 150 + 'px')
const { queryParams, form, rules, sys_show_hide, sys_normal_disable } = toRefs(state)

/** 查询菜单列表 */
function getList(type) {
    loading.value = true
    if (queryParams.value.parentId != undefined || queryParams.value.menuName != undefined) {
        queryParams.value.menuTypeIds = ''
    } else {
        queryParams.value.menuTypeIds = 'M,C'
    }
    listMenu(queryParams.value).then((response) => {
        menuList.value = response.data
        if (type == 1) {
            menuQueryOptions.value = response.data
        }
        loading.value = false
    })
}

/** 查询菜单下拉树结构 */
function getTreeselect() {
    listMenu({ menuTypeIds: 'M,C' }).then((response) => {
        menuOptions.value = response.data
    })
}

/** 取消按钮 */
function cancel() {
    open.value = false
    reset()
}

/** 表单重置 */
function reset() {
    form.value = {
        menuId: undefined,
        parentId: 0,
        menuName: undefined,
        icon: undefined,
        menuType: 'M',
        orderNum: 999,
        isFrame: '0',
        isCache: '0',
        visible: '0',
        status: '0'
    }
    proxy.resetForm('menuRef')
}

/** 选择图标 */
function selected(name) {
    form.value.icon = name
}

/** 搜索按钮操作 */
function handleQuery() {
    getList()
}

/** 重置按钮操作 */
function resetQuery() {
    proxy.resetForm('queryRef')
    handleQuery()
}

/** 新增按钮操作 */
function handleAdd(row) {
    reset()
    getTreeselect()
    if (row != null && row.menuId != undefined) {
        form.value.parentId = row.menuId
    } else {
        form.value.parentId = 0
    }
    open.value = true
    title.value = "新增"
}

/** 展开/折叠操作 */
function toggleExpandAll() {
    // refreshTable.value = false
    isExpandAll.value = !isExpandAll.value
    // nextTick(() => {
    //   refreshTable.value = true
    // })
    const $table = listRef.value
    if ($table) {
        if (isExpandAll.value) {
            $table.setAllTreeExpand(true)
        } else {
            $table.clearTreeExpand()
        }
    }
}

const hasExpandRow = (row) => {
    const $table = listRef.value
    if ($table) {
        return $table.isTreeExpandByRow(row)
    }
    return false
}

/** 修改按钮操作 */
async function handleUpdate(row) {
    reset()
    getTreeselect()
    getMenu(row.menuId).then((response) => {
        form.value = response.data
        open.value = true
        title.value = "编辑"
    })
}

/** 提交按钮 */
function submitForm() {
    proxy.$refs['menuRef'].validate((valid) => {
        if (valid) {
            if (form.value.menuId != undefined) {
                updateMenu(form.value).then(() => {
                    proxy.$modal.msgSuccess('修改成功')
                    open.value = false
                    refreshMenu(form.value.parentId)
                })
            } else {
                addMenu(form.value).then(() => {
                    proxy.$modal.msgSuccess('新增成功')
                    open.value = false
                    refreshMenu(form.value.parentId)
                })
            }
        }
    })
}

/** 删除按钮操作 */
function handleDelete(row) {
    proxy.$modal
        .confirm('是否确认删除名称为"' + row.menuName + '"的数据项?')
        .then(function () {
            return delMenu(row.menuId)
        })
        .then(() => {
            // getList()
            refreshMenu(row.parentId)
            proxy.$modal.msgSuccess('删除成功')
        })
        .catch(() => { })
}

// ******************自定义编辑 start **********************
// 动态ref设置值
const columnRefs = ref([])
const setColumnsRef = (el) => {
    if (el) {
        columnRefs.value.push(el)
    }
}

const editIndex = ref(-1)
// 显示编辑排序
function editCurrRow(rowId) {
    editIndex.value = rowId

    setTimeout(() => {
        columnRefs.value[rowId].focus()
    }, 100)
}

// 保存排序
function handleChangeSort(info) {
    editIndex.value = -1
    proxy.$modal
        .confirm('是否保存数据?')
        .then(function () {
            return changeSort({ value: info.orderNum, id: info.menuId })
        })
        .then(() => {
            handleQuery()
            refreshMenu(info.parentId)
            proxy.$modal.msgSuccess('修改成功')
        })
        .catch(() => {
            handleQuery()
        })
}

// ******************自定义编辑 end **********************
// 刷新懒加载后的数据
function refreshMenu(pid) {
    loading.value = true

    getList()
}

listMenu({ menuTypeIds: 'M,C' }).then((response) => {
    menuQueryOptions.value = response.data
})

// 首次列表加载（只加载一层）
// getList(1)
handleQuery()
</script>
  