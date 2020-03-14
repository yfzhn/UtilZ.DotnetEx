using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace UtilZ.Dotnet.Ex.Base
{
    /// <summary>
    /// 邮件扩展类
    /// </summary>
    public class MailEx
    {
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="smtpHost">邮件服务器主机名称</param>
        /// <param name="userName">发件人登录用户名</param>
        /// <param name="password">发件人登录密码</param>
        /// <param name="subject">主题</param>
        /// <param name="isBodyHtml">邮件格式[true:html;false:Text]</param>
        /// <param name="body">内容</param>
        /// <param name="attachmentFiles">附件文件路径集合</param>
        /// <param name="from">发送件人地址</param>
        /// <param name="to">收件人地址集合</param>
        /// <param name="cc">抄送收件人集合</param>
        /// <param name="bcc">密件抄送收件人集合</param>
        /// <param name="repeatCount">重试次数</param>
        /// <param name="repeatWaitTime">重试间隔，单位/毫秒</param>
        public static void SendMail(string smtpHost, string userName, string password, string subject, bool isBodyHtml, string body, IEnumerable<string> attachmentFiles, MailAddress from, IEnumerable<MailAddress> to, IEnumerable<MailAddress> cc = null, IEnumerable<MailAddress> bcc = null, int repeatCount = 0, int repeatWaitTime = 0)
        {
            if (string.IsNullOrEmpty(smtpHost))
            {
                throw new ArgumentNullException("smtpHost");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("password");
            }

            if (from == null)
            {
                throw new ArgumentNullException("from");
            }

            SmtpClient mailClient = new SmtpClient(smtpHost);
            mailClient.UseDefaultCredentials = false;
            mailClient.Credentials = new System.Net.NetworkCredential(userName, password);
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            MailMessage mailMessage = new MailMessage();
            mailMessage.Priority = MailPriority.Normal;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.SubjectEncoding = Encoding.UTF8;
            mailMessage.Subject = subject;
            //内容
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.Body = body;

            try
            {
                mailMessage.From = from;//发件人
                MailEx.AddMailAddress(mailMessage.To, to);//收件人集合
                MailEx.AddMailAddress(mailMessage.CC, cc);//抄送收件人集合
                MailEx.AddMailAddress(mailMessage.Bcc, bcc);//密件抄送收件人集合

                //添加附件
                if (attachmentFiles != null && attachmentFiles.Count() > 0)
                {
                    foreach (var attachmentFile in attachmentFiles)
                    {
                        Attachment attachment = new Attachment(attachmentFile, System.Net.Mime.MediaTypeNames.Application.Octet);
                        // Add time stamp information for the file.
                        attachment.ContentDisposition.CreationDate = System.IO.File.GetCreationTime(attachmentFile);
                        attachment.ContentDisposition.ModificationDate = System.IO.File.GetLastWriteTime(attachmentFile);
                        attachment.ContentDisposition.ReadDate = System.IO.File.GetLastAccessTime(attachmentFile);
                        // Add the file attachment to this e-mail message.
                        mailMessage.Attachments.Add(attachment);
                    }
                }

                mailClient.Send(mailMessage);
            }
            //catch (SmtpFailedRecipientsException ex)
            //{
            //    int currentRepeatCount = 0;
            //    SmtpFailedRecipientsException smtpFailedRecipientsException = ex;
            //    while (true)
            //    {
            //        currentRepeatCount++;
            //        for (int i = 0; i < ex.InnerExceptions.Length; i++)
            //        {
            //            SmtpStatusCode status = ex.InnerExceptions[i].StatusCode;
            //            if (status == SmtpStatusCode.MailboxBusy || status == SmtpStatusCode.MailboxUnavailable)
            //            {
            //                if (repeatWaitTime > 0)
            //                {
            //                    System.Threading.Thread.Sleep(repeatWaitTime);
            //                }

            //                try
            //                {
            //                    mailClient.Send(mailMessage);
            //                }
            //                catch (SmtpFailedRecipientsException exi)
            //                {
            //                    if (currentRepeatCount < repeatCount)
            //                    {
            //                        throw exi;
            //                    }
            //                }
            //            }
            //        }


            //    }
            //}
            finally
            {
                for (int i = 0; i < mailMessage.Attachments.Count; i++) //释放占用的资源
                {
                    mailMessage.Attachments[i].Dispose();
                }
            }
        }

        /// <summary>
        /// 将源邮件地址添加到目标邮件地址集合中
        /// </summary>
        /// <param name="targetCollection">目标邮件地址集合</param>
        /// <param name="sourceMailAddrs">源邮件地址集合</param>
        private static void AddMailAddress(MailAddressCollection targetCollection, IEnumerable<MailAddress> sourceMailAddrs)
        {
            if (sourceMailAddrs == null || sourceMailAddrs.Count() == 0)
            {
                return;
            }

            foreach (var ma in sourceMailAddrs)
            {
                targetCollection.Add(ma);
            }
        }
    }
}
