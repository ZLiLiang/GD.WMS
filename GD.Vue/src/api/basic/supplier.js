import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

/**
 * 查询所有数据
 * @param {查询参数} query 
 * @returns 
 */
export function getAll(query) {
    return request({
        url: '/basic/supplier',
        method: 'get',
        params: query
    })
}

/**
 * 根据Id查询供应商信息
 * @param {供应商Id} supplierId 
 * @returns 
 */
export function getSupplierInfo(supplierId) {
    return request({
        url: '/basic/supplier/' + praseStrZero(supplierId),
        method: 'get'
    })
}

/**
 * 新增供应商信息
 * @param {供应商信息} entity 
 * @returns 
 */
export function addSupplierInfo(entity) {
    return request({
        url: '/basic/supplier/add',
        method: 'post',
        params: entity
    })
}

/**
 * 编辑供应商信息
 * @param {供应商Id} supplierId 
 * @param {供应商信息} entity 
 * @returns 
 */
export function editSupplierInfo(supplierId, entity) {
    return request({
        url: '/basic/supplier/edit/' + praseStrZero(supplierId),
        method: 'post',
        params: entity
    })
}

/**
 * 根据Id删除供应商信息
 * @param {供应商Id} supplierId 
 * @returns 
 */
export function deleteById(supplierId) {
    return request({
        url: '/basic/supplier/' + praseStrZero(supplierId),
        method: 'delete'
    })
}

/**
 * 导出供应商报表
 */
export async function exportSupplier() {
    await downFile('/basic/supplier/export')
}

/**
 * 导出供应商模板
 */
export async function exportTemplate() {
    await downFile('/basic/supplier/exportTemplate')
}

