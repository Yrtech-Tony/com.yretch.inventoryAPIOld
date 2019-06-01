﻿using com.yrtech.InventoryAPI.Common;
using com.yrtech.InventoryAPI.DTO;
using com.yrtech.InventoryDAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace com.yrtech.InventoryAPI.Service
{
    public class AnswerService
    {
        Inventory db = new Inventory();
        MasterService masterService = new MasterService();
        AccountService accountService = new AccountService();

        /// <summary>
        /// </summary>
        /// <param name="projectCode"></param>
        /// <param name="shopCode"></param>
        /// <param name="allChk"></param>
        /// <param name="vinCode"></param>
        /// <returns></returns>
        public List<AnswerDto> GetShopAnswerList(string projectCode, string shopCode,string allChk,string vinCode)
        {

            SqlParameter[] para = new SqlParameter[] { new SqlParameter("@ProjectCode", projectCode),
                                                       new SqlParameter("@ShopCode", shopCode),
                                                       new SqlParameter("@AllChk", allChk),
                                                       new SqlParameter("@VinCode", vinCode)};
            Type t = typeof(AnswerDto);
            string sql = "";
            sql = @"SELECT VinCode,VinCode8,PhotoName,AddChk,Remark 
                    FROM Answer 
                    WHERE ProjectCode=@ProjectCode  ";
            if (!string.IsNullOrEmpty(shopCode))
            {
                sql += " AND ShopCode = @ShopCode";
            }
            if (allChk=="N") // 不是查询全部的时候，志查询未拍照的清单
            {
                sql += " AND AddChk='N' AND (VinPhotoName IS NULL OR VinPhotoName='') AND (remark is null or remark='')";
            }
            if (!string.IsNullOrEmpty(vinCode))
            {
                sql += " AND VinCode LIKE '%'+@VinCode+'%'";
            }
            sql += " Order By vincode8";
            return db.Database.SqlQuery(t, sql, para).Cast<AnswerDto>().ToList();
        }
        public void SaveShopAnswer(Answer answer)
        {
            Answer findOne = db.Answer.Where(x => (x.ProjectCode == answer.ProjectCode&&x.ShopCode==answer.ShopCode&&answer.VinCode==x.VinCode)).FirstOrDefault();
            if (findOne == null)
            {
                answer.InDateTime = DateTime.Now;
                answer.VinCode8 = answer.VinCode.Substring(answer.VinCode.Length - 8);
                answer.AddChk = "Y";
            }
            else
            {
                findOne.PhotoName = answer.PhotoName;
                findOne.Remark = answer.Remark;
            }
            db.SaveChanges();
        }
    }
}