﻿using Model;
using PublicDefined;
using RuleUtility;
using ServicesInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Tools;

namespace XlettlerRealization
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public class UserService : IUserInterface
    {
        public bool BindRealName(string AccountID, string RealName, string IdCardNum, out string result, out string errMsg)
        {
            errMsg = string.Empty;
            result = string.Empty;
            try
            {
                ModelContainer container = new ModelContainer();
                SESENT_USERS _USERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                if (_USERS == null) { errMsg = "绑定的账户不存在!";return false; }
                _USERS.RealName = RealName;
                _USERS.IDCardNum = IdCardNum;
                container.Entry<SESENT_USERS>(_USERS).State = System.Data.Entity.EntityState.Modified;
                bool ret= container.SaveChanges() > 0;
                if (ret)
                    result = "绑定成功!";
                else
                    errMsg = "绑定失败!";
                return true;
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"绑定实名账号失败:{err.Message}");
                errMsg = "未知错误!";
                return false;
            }
        }
        public bool QueryUserBindRealName(string AccountID, out IDictionary<string, object> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            try
            {
                using (ModelContainer container = new ModelContainer())
                {
                    SESENT_USERS _USERS = container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (_USERS == null) { errMsg = "未查询到该用户信息!"; return false; }
                    if (!string.IsNullOrEmpty(_USERS.RealName) && !string.IsNullOrEmpty(_USERS.IDCardNum))
                        result.Add("Status", 1);
                    else
                        result.Add("Status",0);
                    return true;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"查询绑定实名账号失败:账号{AccountID}错误:{err.Message}");
                errMsg = "未知错误!";
                return false;
            }
        }
        public bool CheckReister(string Phone, string AccountID, out string Msg)
        {
            try
            {
                if (string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(AccountID)) { Msg = "参数错误!"; return false; }
                else
                {
                    bool exit = false;
                    ModelContainer container = new ModelContainer();
                    if (!string.IsNullOrEmpty(Phone) && !string.IsNullOrEmpty(AccountID)) { Msg = "未指明检测项!"; return exit; }
                    else if (!string.IsNullOrEmpty(Phone))
                    {
                        exit = container.SESENT_USERS.Where(a => a.Phone == Phone).FirstOrDefault() == null;
                        if (exit) Msg = "该手机号未注册!";
                        else Msg = "该手机号已注册!";
                        return exit;
                    }
                    else
                    {
                        exit = Verification.VerifyWithAccountID(AccountID);
                        if (exit) { Msg = "账号范围英文和数字"; return false; }
                        exit = container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault() == null;
                        if (exit) Msg = "该账号未注册!";
                        else Msg = "该账号已注册!";
                        return exit;
                    }
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"检验注册账号失败:{err.Message}");
                Msg = "未知错误!";
                return false;
            }
        }

        public bool FindLoginPwd(string Phone, string Code, string passWord, out string errMsg)
        {
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer())
            {
                try
                {
                    bool ret = Code == RedisHelper.GetManger().Get(CacheKey.GenerateCachePhoneCode(Phone, IPhoneCodeType.FindLoginPwd));
                    if (!ret) { errMsg = "验证码错误!"; return false; }
                    SESENT_USERS _USERS = container.SESENT_USERS.Where(a => a.Phone == Phone).FirstOrDefault();
                    if (_USERS == null) { errMsg = "不存在该用户!"; return false; }
                    _USERS.userPwd = passWord;
                    return container.SaveChanges() > 0;
                }
                catch (Exception err)
                {
                    LogTool.LogWriter.WriteError($"修改密码错误:{err.Message},错误账号:{Phone}");
                    return false;
                }
            }
        }

        public bool Login(string userName, string passWord, string Phone, string Code, LoginType type, out string errMsg)
        {
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer())
            {
                try
                {
                    if (type == LoginType.Account)
                    {
                        SESENT_USERS user = container.SESENT_USERS.Where(a => a.AccountID == userName && a.userPwd == passWord).FirstOrDefault();
                        if (user != null) { return true; }
                        else { errMsg = "账号或密码不正确。"; return false; }
                    }
                    else if (type == LoginType.PhoneCode)
                    {
                        string code = RedisHelper.GetManger().Get(CacheKey.GenerateCachePhoneCode(Phone, IPhoneCodeType.Login));
                        if (string.IsNullOrEmpty(code) || code != Code) { errMsg = "验证码不正确!"; return false; } else { return true; }
                    }
                    else
                    {
                        errMsg = "暂未开放该登陆类型!"; return false;
                    }
                }
                catch (Exception err)
                {
                    errMsg = "登陆失败!请联系客服。";
                    LogTool.LogWriter.WriteError($"用户登陆失败！【用户编号:{userName},用户手机号:{Phone},登陆密码:{passWord},错误信息:{err.Message}】");
                    return false;
                }
            }
        }

        public bool QueryUserInfo(string AccountID, out IDictionary<string, object> result, out string errMsg)
        {
            result = null;
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer())
            {
                try
                {
                    SESENT_USERS uSERS = container.SESENT_USERS.Where(a => a.AccountID == AccountID || a.Phone == AccountID).FirstOrDefault();
                    if (uSERS == null) { errMsg = "未查询到账户信息!"; return false; }
                    result = new Dictionary<string, object>();
                    result.Add("Lv", uSERS.Lv);
                    result.Add("UseAmount", uSERS.UseAmount);
                    result.Add("userName", uSERS.userName);
                    result.Add("AgentMoney", uSERS.AgentMoney);
                    result.Add("AccountID", string.IsNullOrEmpty(uSERS.AccountID) ? uSERS.Phone : uSERS.AccountID);
                    result.Add("userType", uSERS.userType);
                    result.Add("Consumption", uSERS.Consumption);
                    result.Add("Recharge", uSERS.Recharge);
                    result.Add("Phone", uSERS.Phone);
                    result.Add("HeadImg", uSERS.HeadImg==null?null:Convert.ToBase64String(uSERS.HeadImg));
                    if (uSERS.userType ==(short)PublicDefined.UserType.angency)
                    {
                        result.Add("IsAngency", true);
                    }
                    return true;
                }
                catch (Exception err)
                {
                    errMsg = "查询失败!";
                    LogTool.LogWriter.WriteError("查询账户信息失败", err);
                    return false;
                }
            }
        }

        public bool Register(string AccountID, string passWord, string Phone, string agencyID, string Code, UserType type, out string errMsg)
        {
            errMsg = string.Empty;
            using (ModelContainer container = new ModelContainer())
            {
                try
                {
                    string code = RedisHelper.GetManger().Get(CacheKey.GenerateCachePhoneCode(Phone, IPhoneCodeType.Register));
                    if (string.IsNullOrEmpty(code) || code != Code) { errMsg = "验证码错误!"; return false; }
                    SESENT_USERS uSERS = container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (uSERS != null) { errMsg = "该账户已被注册。"; return false; }
                    if (!string.IsNullOrEmpty(agencyID))
                    {
                        SESENT_USERS angency = container.SESENT_USERS.Where(a => a.AccountID == agencyID && a.userType == (short)UserType.angency).FirstOrDefault();
                        if (angency == null) { errMsg = "代理编号错误!"; return false; }
                    }
                    uSERS = new SESENT_USERS()
                    {
                        AccountID = AccountID,
                        Phone = Phone,
                        SuperiorAgent = agencyID,
                        userPwd = passWord,
                        userPayPwd = "",
                        userType = (short)type,
                        UseAmount = 0,
                        AgentMoney = 0,

                    };
                    container.SESENT_USERS.Add(uSERS);
                    bool ret = container.SaveChanges() > 0;
                    if (!ret) errMsg = "注册失败!";
                    return ret;
                }
                catch (Exception err)
                {
                    errMsg = "注册失败!";
                    LogTool.LogWriter.WriteError("用户注册失败!" + err.Message);
                    return false;
                }
            }
        }

        public bool SendUserCode(string Phone, IPhoneCodeType type, out string errMsg)
        {
            return ActiveMQHelper.GetManger().SendMessage(Phone, type, out errMsg);
        }

        public bool UserUploadHeadImg(string AccountID, byte[] img, out string result, out string errMsg)
        {
            result = string.Empty;
            errMsg = string.Empty;
            try
            {
                using (Model.ModelContainer container=new ModelContainer())
                {
                    SESENT_USERS _USERS=container.SESENT_USERS.Where(a => a.AccountID == AccountID).FirstOrDefault();
                    if (_USERS == null) { errMsg = "未查询到该账户信息!"; return false; }
                    _USERS.HeadImg = img;
                    if (_USERS.HeadImg != null) container.Entry(_USERS).State = System.Data.Entity.EntityState.Modified;
                    else container.Entry(_USERS).State = System.Data.Entity.EntityState.Added;
                    bool ret=container.SaveChanges() > 0;
                    if (ret) result = "上传成功!";
                    else errMsg = "上传失败!";
                    return ret;
                }
            }
            catch (Exception err)
            {
                LogTool.LogWriter.WriteError($"用户图片上传错误:{err}");
                errMsg = "未知错误!";
                return false;
            }
        }
    }
}
