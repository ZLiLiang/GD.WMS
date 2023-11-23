import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/basic/location'

/**
 * 分页查询所有数据
 * @param {查询参数} query 
 * @returns 封装请求
 */
export function getAllInfo(query) {
    return request({
        url: baseUrl,
        method: 'get',
        params: query
    })
}

/**
 * 获取仓库选项
 * @returns 返回请求
 */
export function getWarehouseOptions() {
    let url = `${baseUrl}/warehouseOptions`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 获取库区选项
 * @param {仓库id} id 
 * @returns 返回请求
 */
export function getRegionOptions(id) {
    let url = `${baseUrl}/regionOptions/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 通过id查询单个库位信息
 * @param {库位ID} id 
 * @returns 封装请求
 */
export function getInfo(id) {
    let url = `${baseUrl}/get/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'get'
    })
}

/**
 * 新增库位
 * @param {库位实体} entity 
 * @returns 封装请求
 */
export function addInfo(entity) {
    let url = `${baseUrl}/add`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 根据库位id修改库位信息
 * @param {库位ID} id 
 * @param {库位实体} entity 
 * @returns 封装请求
 */
export function editInfo(id, entity) {
    let url = `${baseUrl}/edit/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'post',
        data: entity
    })
}

/**
 * 根据id删除库位信息
 * @param {库位ID} id 
 * @returns 封装请求
 */
export function deleteInfo(id) {
    let url = `${baseUrl}/delete/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'delete'
    })
}

/**
 * 导出库区信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/export`
    await downFile(url)
}