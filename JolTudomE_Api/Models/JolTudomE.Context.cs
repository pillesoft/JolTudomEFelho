﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JolTudomE_Api.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class JolTudomEEntities : DbContext
    {
        public JolTudomEEntities()
            : base("name=JolTudomEEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Sessions> Sessions { get; set; }
        public virtual DbSet<Person> Person { get; set; }
    
        public virtual int usp_AddNewUser(string username, string prefix, string lastname, string middlename, string firstname, string password, Nullable<byte> role)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var prefixParameter = prefix != null ?
                new ObjectParameter("prefix", prefix) :
                new ObjectParameter("prefix", typeof(string));
    
            var lastnameParameter = lastname != null ?
                new ObjectParameter("lastname", lastname) :
                new ObjectParameter("lastname", typeof(string));
    
            var middlenameParameter = middlename != null ?
                new ObjectParameter("middlename", middlename) :
                new ObjectParameter("middlename", typeof(string));
    
            var firstnameParameter = firstname != null ?
                new ObjectParameter("firstname", firstname) :
                new ObjectParameter("firstname", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            var roleParameter = role.HasValue ?
                new ObjectParameter("role", role) :
                new ObjectParameter("role", typeof(byte));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_AddNewUser", usernameParameter, prefixParameter, lastnameParameter, middlenameParameter, firstnameParameter, passwordParameter, roleParameter);
        }
    
        public virtual ObjectResult<usp_Authenticate_Result> usp_Authenticate(string username, string password)
        {
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            var passwordParameter = password != null ?
                new ObjectParameter("password", password) :
                new ObjectParameter("password", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_Authenticate_Result>("usp_Authenticate", usernameParameter, passwordParameter);
        }
    
        public virtual int usp_SessionsCleanup(Nullable<int> timeout)
        {
            var timeoutParameter = timeout.HasValue ?
                new ObjectParameter("timeout", timeout) :
                new ObjectParameter("timeout", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_SessionsCleanup", timeoutParameter);
        }
    
        public virtual int usp_CleanupTests(Nullable<int> timeout)
        {
            var timeoutParameter = timeout.HasValue ?
                new ObjectParameter("timeout", timeout) :
                new ObjectParameter("timeout", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_CleanupTests", timeoutParameter);
        }
    
        public virtual ObjectResult<usp_SearchUser_Result> usp_SearchUser(Nullable<int> roleid, string prefix, string firstname, string middlename, string lastname, string username)
        {
            var roleidParameter = roleid.HasValue ?
                new ObjectParameter("roleid", roleid) :
                new ObjectParameter("roleid", typeof(int));
    
            var prefixParameter = prefix != null ?
                new ObjectParameter("prefix", prefix) :
                new ObjectParameter("prefix", typeof(string));
    
            var firstnameParameter = firstname != null ?
                new ObjectParameter("firstname", firstname) :
                new ObjectParameter("firstname", typeof(string));
    
            var middlenameParameter = middlename != null ?
                new ObjectParameter("middlename", middlename) :
                new ObjectParameter("middlename", typeof(string));
    
            var lastnameParameter = lastname != null ?
                new ObjectParameter("lastname", lastname) :
                new ObjectParameter("lastname", typeof(string));
    
            var usernameParameter = username != null ?
                new ObjectParameter("username", username) :
                new ObjectParameter("username", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_SearchUser_Result>("usp_SearchUser", roleidParameter, prefixParameter, firstnameParameter, middlenameParameter, lastnameParameter, usernameParameter);
        }
    
        public virtual ObjectResult<usp_GetCourses_Result> usp_GetCourses()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetCourses_Result>("usp_GetCourses");
        }
    
        public virtual ObjectResult<usp_GetTopics_Result> usp_GetTopics(Nullable<int> courseid)
        {
            var courseidParameter = courseid.HasValue ?
                new ObjectParameter("courseid", courseid) :
                new ObjectParameter("courseid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetTopics_Result>("usp_GetTopics", courseidParameter);
        }
    
        public virtual ObjectResult<usp_Statistics_Result> usp_Statistics(Nullable<int> person, Nullable<int> callerid, Nullable<int> roleid)
        {
            var personParameter = person.HasValue ?
                new ObjectParameter("person", person) :
                new ObjectParameter("person", typeof(int));
    
            var calleridParameter = callerid.HasValue ?
                new ObjectParameter("callerid", callerid) :
                new ObjectParameter("callerid", typeof(int));
    
            var roleidParameter = roleid.HasValue ?
                new ObjectParameter("roleid", roleid) :
                new ObjectParameter("roleid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_Statistics_Result>("usp_Statistics", personParameter, calleridParameter, roleidParameter);
        }
    
        public virtual ObjectResult<usp_Eval_Result> usp_Eval(Nullable<int> testid, Nullable<int> person, Nullable<int> callerid, Nullable<int> roleid)
        {
            var testidParameter = testid.HasValue ?
                new ObjectParameter("testid", testid) :
                new ObjectParameter("testid", typeof(int));
    
            var personParameter = person.HasValue ?
                new ObjectParameter("person", person) :
                new ObjectParameter("person", typeof(int));
    
            var calleridParameter = callerid.HasValue ?
                new ObjectParameter("callerid", callerid) :
                new ObjectParameter("callerid", typeof(int));
    
            var roleidParameter = roleid.HasValue ?
                new ObjectParameter("roleid", roleid) :
                new ObjectParameter("roleid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_Eval_Result>("usp_Eval", testidParameter, personParameter, calleridParameter, roleidParameter);
        }
    
        public virtual int usp_CancelTest(Nullable<int> testid, Nullable<int> person)
        {
            var testidParameter = testid.HasValue ?
                new ObjectParameter("testid", testid) :
                new ObjectParameter("testid", typeof(int));
    
            var personParameter = person.HasValue ?
                new ObjectParameter("person", person) :
                new ObjectParameter("person", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_CancelTest", testidParameter, personParameter);
        }
    
        public virtual int usp_CheckedAnswer(Nullable<int> testid, Nullable<int> questionid, Nullable<int> answerid, Nullable<bool> complete)
        {
            var testidParameter = testid.HasValue ?
                new ObjectParameter("testid", testid) :
                new ObjectParameter("testid", typeof(int));
    
            var questionidParameter = questionid.HasValue ?
                new ObjectParameter("questionid", questionid) :
                new ObjectParameter("questionid", typeof(int));
    
            var answeridParameter = answerid.HasValue ?
                new ObjectParameter("answerid", answerid) :
                new ObjectParameter("answerid", typeof(int));
    
            var completeParameter = complete.HasValue ?
                new ObjectParameter("complete", complete) :
                new ObjectParameter("complete", typeof(bool));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_CheckedAnswer", testidParameter, questionidParameter, answeridParameter, completeParameter);
        }
    
        public virtual ObjectResult<usp_GetAllUsers_Result> usp_GetAllUsers(Nullable<int> roleid, Nullable<int> sroleid)
        {
            var roleidParameter = roleid.HasValue ?
                new ObjectParameter("roleid", roleid) :
                new ObjectParameter("roleid", typeof(int));
    
            var sroleidParameter = sroleid.HasValue ?
                new ObjectParameter("sroleid", sroleid) :
                new ObjectParameter("sroleid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_GetAllUsers_Result>("usp_GetAllUsers", roleidParameter, sroleidParameter);
        }
    
        public virtual int usp_AddEvent(Nullable<int> testid, Nullable<int> eventid)
        {
            var testidParameter = testid.HasValue ?
                new ObjectParameter("testid", testid) :
                new ObjectParameter("testid", typeof(int));
    
            var eventidParameter = eventid.HasValue ?
                new ObjectParameter("eventid", eventid) :
                new ObjectParameter("eventid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("usp_AddEvent", testidParameter, eventidParameter);
        }
    
        public virtual ObjectResult<usp_ContineTest_Result> usp_ContineTest(Nullable<int> personid)
        {
            var personidParameter = personid.HasValue ?
                new ObjectParameter("personid", personid) :
                new ObjectParameter("personid", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<usp_ContineTest_Result>("usp_ContineTest", personidParameter);
        }
    }
}
