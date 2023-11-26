import request from '@/utils/request'
import { praseStrZero } from '@/utils/ruoyi'
import { downFile } from '@/utils/request'

const baseUrl = '/receive/asn'

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
 * 根据asn信息id修改asn信息
 * @param {asn信息ID} id 
 * @param {asn信息实体} entity 
 * @returns 封装请求
 */
export function editInfo(id, entity) {
    // let url = `${baseUrl}/edit/${praseStrZero(id)}`
    // return request({
    //     url: url,
    //     method: 'post',
    //     data: entity
    // })
}

/**
 * 取消分拣
 * @param {行数据Id} id 
 */
export function cancelSort(id) {
    let url = `${baseUrl}/cancelSort/${praseStrZero(id)}`
    return request({
        url: url,
        method: 'put'
    })
}

/**
 * 导出待上架信息
 */
export async function exportAllInfo() {
    let url = `${baseUrl}/actualExport`
    await downFile(url)
}