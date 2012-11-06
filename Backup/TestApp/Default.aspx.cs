using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Reflection;
using stillbreathing.co.uk.ActionManager;

namespace TestApp
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Generic tests

            // tests without the object parameter

            // same namespace, static
            ActionMethod Test1 = new ActionMethod("TestApp.StaticMethods.WithoutObject");
            ActionManager.AddMethod("TestAction", Test1);

            // same namespace, instantiated
            ActionMethod Test2 = new ActionMethod("TestApp.Methods.WithoutObject", false);
            ActionManager.AddMethod("TestAction", Test2);

            // different namespace, static
            ActionMethod Test3 = new ActionMethod("Namespace1.StaticMethods.WithoutObject");
            ActionManager.AddMethod("TestAction", Test3);

            // different namespace, instantiated
            ActionMethod Test4 = new ActionMethod("Namespace2.Methods.WithoutObject", false);
            ActionManager.AddMethod("TestAction", Test4);

            // tests with the object parameter

            // same namespace, static
            ActionMethod Test5 = new ActionMethod("TestApp.StaticMethods.WithObject", true, true);
            ActionManager.AddMethod("TestActionWithObject", Test5);

            // same namespace, instantiated
            ActionMethod Test6 = new ActionMethod();
            Test6.Namespace = "TestApp";
            Test6.ClassName = "Methods";
            Test6.MethodName = "WithObject";
            Test6.AcceptsParameters = true;
            Test6.IsStatic = false;
            ActionManager.AddMethod("TestActionWithObject", Test6);

            // different namespace, static
            ActionMethod Test7 = new ActionMethod();
            Test7.Namespace = "Namespace1";
            Test7.ClassName = "StaticMethods";
            Test7.MethodName = "WithObject";
            Test7.AcceptsParameters = true;
            ActionManager.AddMethod("TestActionWithObject", Test7);

            // different namespace, instantiated
            ActionMethod Test8 = new ActionMethod();
            Test8.Namespace = "Namespace2";
            Test8.ClassName = "Methods";
            Test8.MethodName = "WithObject";
            Test8.AcceptsParameters = true;
            Test8.IsStatic = false;
            ActionManager.AddMethod("TestActionWithObject", Test8);

            #endregion

            // run the test method
            TestMethod();
        }

        /// <summary>
        /// Run a simple method with actions that can be used
        /// </summary>
        private void TestMethod()
        {
            // register the new action
            ActionMethod AddPerson = new ActionMethod();
            AddPerson.Namespace = "TestApp";
            AddPerson.ClassName = "PersonMethods";
            AddPerson.MethodName = "OnAddPerson";
            AddPerson.IsStatic = false;
            AddPerson.AcceptsParameters = true;
            AddPerson.ThrowOnException = true;
            ActionManager.AddMethod("AddPerson", AddPerson);

            // create a new datatable
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");
            dt.Columns.Add("age");
            dt.Columns.Add("extra");

            // add some rows, calling the AddRow action
            object[] person1 = new object[] { 1, "Jim", 33, null };
            dt.Rows.Add(person1);
            ActionManager.PerformAction("AddPerson", dt.Rows[dt.Rows.Count - 1]);

            object[] person2 = new object[] { 2, "Dave", 29, null };
            dt.Rows.Add(person2);
            ActionManager.PerformAction("AddPerson", dt.Rows[dt.Rows.Count - 1]);

            object[] person3 = new object[] { 4, "Harriet", 45, null };
            dt.Rows.Add(person3);
            ActionManager.PerformAction("AddPerson", dt.Rows[dt.Rows.Count - 1]);

            object[] person4 = new object[] { 5, "Nicola", 19, null };
            dt.Rows.Add(person4);
            ActionManager.PerformAction("AddPerson", dt.Rows[dt.Rows.Count-1]);

            PeopleTable.DataSource = dt;
            PeopleTable.DataBind();
        }
    }

    #region PersonMethods class

    class PersonMethods
    {
        /// <summary>
        /// When a person DataRow is added to a DataTable
        /// </summary>
        /// <param name="Person"></param>
        /// <returns></returns>
        public object OnAddPerson(object Person)
        {
            DataRow PersonRow = (DataRow)Person;
            PersonRow["extra"] = "";
            if (Convert.ToInt16(PersonRow["age"]) < 22)
            {
                PersonRow["extra"] += "Junior (age is below 22)";
            }
            if (Convert.ToInt16(PersonRow["age"]) > 44)
            {
                PersonRow["extra"] += "Senior (age over 44)";
            }
            if (PersonRow["name"].ToString() == "Jim")
            {
                PersonRow["extra"] = "Manager (name is 'Jim')";
            }
            return PersonRow;
        }
    }

    #endregion

    #region Generic test classes

    public static class StaticMethods
    {
        public static void WithoutObject()
        {
            HttpContext.Current.Response.Write("<p>TestApp.StaticMethods.WithoutObject()</p>");
        }

        public static object WithObject(object Object)
        {
            HttpContext.Current.Response.Write("<p>" + ((string)Object) + " amended by TestApp.StaticMethods.WithObject()</p>");
            return Object;
        }
    }

    public class Methods
    {
        public void WithoutObject()
        {
            HttpContext.Current.Response.Write("<p>TestApp.Methods.WithoutObject()</p>");
        }

        public object WithObject(object Object)
        {
            HttpContext.Current.Response.Write("<p>" + ((string)Object) + " amended by TestApp.Methods.WithObject()</p>");
            return Object;
        }
    }

    #endregion
}

#region Generic test namespaces

namespace Namespace1
{
    public static class StaticMethods
    {
        public static void WithoutObject()
        {
            HttpContext.Current.Response.Write("<p>Namespace1.StaticMethods.WithoutObject()</p>");
        }

        public static object WithObject(object Object)
        {
            HttpContext.Current.Response.Write("<p>" + ((string)Object) + " amended by Namespace1.StaticMethods.WithObject()</p>");
            return Object;
        }
    }
}

namespace Namespace2
{
    public class Methods
    {
        public void WithoutObject()
        {
            HttpContext.Current.Response.Write("<p>Namespace2.Methods.WithoutObject()</p>");
        }

        public static object WithObject(object Object)
        {
            HttpContext.Current.Response.Write("<p>" + ((string)Object) + " amended by Namespace2.Methods.WithObject()</p>");
            return Object;
        }
    }
}

#endregion

