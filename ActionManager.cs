using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

namespace stillbreathing.co.uk.ActionManager
{
    /// <summary>
    /// This class allows registered methods to be instantiated and run when named actions occur.
    /// For more details see http://www.stillbreathing.co.uk/projects/actionmanager
    /// </summary>
    public static class ActionManager
    {
        /// <summary>
        /// The list of registered methods
        /// </summary>
        public static List<ActionMethod> Methods = new List<ActionMethod>();

        /// <summary>
        /// Add an ActionMethod (with lowest priority) to be called when an Action is performed
        /// </summary>
        /// <param name="ActionName">The name of the Action that will trigger the ActionMethod</param>
        /// <param name="Method">The ActionMethod to run</param>
        public static void AddMethod(string actionName, ActionMethod method)
        {
            // set the method assembly, if it is not manually set
            if (method.AssemblyName == null)
            {
                method.AssemblyName = Assembly.GetCallingAssembly().FullName;
            }

            ActionManager.AddMethod(actionName, method, 0);
        }

        /// <summary>
        /// Add an ActionMethod to be called when an Action is performed
        /// </summary>
        /// <param name="ActionName">The name of the Action that will trigger the ActionMethod</param>
        /// <param name="Method">The ActionMethod to run</param>
        /// <param name="priority">The priority of the ActionMethod</param>
        public static void AddMethod(string actionName, ActionMethod method, int priority)
        {
            // set the action name
            method.ActionName = actionName;

            // set the action priority
            method.Priority = priority;

            // set the method assembly, if it is not manually set
            if (method.AssemblyName == null)
            {
                method.AssemblyName = Assembly.GetCallingAssembly().FullName;
            }

            // if the method already exists for this action, delete it
            if (Methods.FirstOrDefault(m => m.ActionName == method.ActionName && m.Namespace == method.Namespace && m.ClassName == method.ClassName && m.MethodName == method.MethodName) != null)
            {
                ActionMethod foundMethod = Methods.FirstOrDefault(m => m.ActionName == method.ActionName && m.Namespace == method.Namespace && m.ClassName == method.ClassName && m.MethodName == method.MethodName);
                Methods.Remove(foundMethod);
            }

            // add this method
            Methods.Add(method);
        }

        /// <summary>
        /// Perform an action
        /// </summary>
        /// <param name="Name"></param>
        public static void PerformAction(string name)
        {
            ActionManager.PerformAction(name, null);
        }

        /// <summary>
        /// Perform an action and return the object
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Object"></param>
        /// <returns></returns>
        public static object PerformAction(string name, params object[] parameterObjects)
        {
            // get the object to return
            object initialReturnObject = null;
            object returnObject = null;
            if (parameterObjects != null && parameterObjects.Length > 0)
            {
                initialReturnObject = parameterObjects[0];
                returnObject = parameterObjects[0];
            }

            // get the methods for this action, ordered by the priority descending
            List<ActionMethod> foundMethods = Methods.FindAll(m => m.ActionName == name).OrderByDescending(m => m.Priority).ToList();
            foreach (ActionMethod method in foundMethods)
            {
                // get the type of the method class
                Type classType = Type.GetType(method.Namespace + "." + method.ClassName + "," + method.AssemblyName);

                // if the type can be found
                if (classType != null)
                {
                    // if the method can be found
                    MemberInfo[] memberInfo = classType.GetMember(method.MethodName);
                    if (memberInfo != null && memberInfo.Length > 0 && memberInfo[0].MemberType == MemberTypes.Method)
                    {
                        // get the method details
                        MethodInfo methodInfo = (MethodInfo)memberInfo[0];
                        int parameters = methodInfo.GetParameters().Length;
                        Type returnType = methodInfo.ReturnType;
                        // if the method does not accept parameters
                        if (parameters == 0)
                        {
                            parameterObjects = null;
                        }

                        // if this is a static method
                        if (method.IsStatic)
                        {
                            try
                            {
                                if (methodInfo.ReturnType == typeof(void))
                                {
                                    classType.InvokeMember(method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, parameterObjects);
                                }
                                else
                                {
                                    returnObject = (object)classType.InvokeMember(method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, parameterObjects);
                                }
                            }
                            catch (Exception ex)
                            {
                                if (method.ThrowOnException)
                                {
                                    throw ex;
                                }
                            }
                        }

                        // if the class must be instantiated
                        if (!method.IsStatic)
                        {
                            Assembly Assembly = Assembly.Load(method.AssemblyName);
                            object instantiatedObject = Assembly.CreateInstance(method.Namespace + "." + method.ClassName);
                            if (instantiatedObject != null)
                            {
                                try
                                {
                                    if (methodInfo.ReturnType == typeof(void))
                                    {
                                        classType.InvokeMember(method.MethodName, BindingFlags.InvokeMethod, null, instantiatedObject, parameterObjects);
                                    }
                                    else
                                    {
                                        returnObject = (object)classType.InvokeMember(method.MethodName, BindingFlags.InvokeMethod, null, instantiatedObject, parameterObjects);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    if (method.ThrowOnException)
                                    {
                                        throw;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // if the return object is null, return the unmodified object
            if (returnObject == null)
            {
                returnObject = initialReturnObject;
            }

            return returnObject;
        }
    }

    /// <summary>
    /// A method to be called when an action is performed
    /// </summary>
    public class ActionMethod
    {
        /// <summary>
        /// Create a new ActionMethod
        /// </summary>
        public ActionMethod()
        {

        }
        /// <summary>
        /// Create a new ActionMethod
        /// </summary>
        /// <param name="targetMethod">A string reference to the target method (e.g. "Namespace.Namespace.Class.Method")</param>
        public ActionMethod(string targetMethod)
        {
            CreateActionMethod(targetMethod, false);
        }
        /// <summary>
        /// Create a new ActionMethod
        /// </summary>
        /// <param name="targetMethod">A string reference to the target method (e.g. "Namespace.Namespace.Class.Method")</param>
        /// <param name="isStatic">Whether the target class and method is static (true), or needs to be instantiated (false)</param>
        public ActionMethod(string targetMethod, bool isStatic)
        {
            CreateActionMethod(targetMethod, isStatic);
        }

        /// <summary>
        /// Create a new ActionMethod
        /// </summary>
        /// <param name="targetMethod">A string reference to the target method (e.g. "Namespace.Namespace.Class.Method")</param>
        /// <param name="isStatic">Whether the target class and method is static (true), or needs to be instantiated (false)</param>
        public void CreateActionMethod(string targetMethod, bool isStatic)
        {
            string[] methodParts = targetMethod.Split('.');
            if (methodParts.Length > 2)
            {
                this.ClassName = methodParts[methodParts.Length - 2];
                this.MethodName = methodParts[methodParts.Length - 1];
                this.Namespace = targetMethod.Replace("." + this.ClassName + "." + this.MethodName, "");
                this.IsStatic = isStatic;
            }
        }

        /// <summary>
        /// The name of the action this method will be called for
        /// </summary>
        public string ActionName;

        /// <summary>
        /// The priority of this method, higher numbers are executed first
        /// </summary>
        public int Priority;

        /// <summary>
        /// Whether this method is static, or needs instantiating
        /// </summary>
        public bool IsStatic = true;

        /// <summary>
        /// The name of the assembly containing this method
        /// </summary>
        public string AssemblyName = null;

        /// <summary>
        /// The namespace of this class
        /// </summary>
        public string Namespace = null;

        /// <summary>
        /// The name of the class to be called 
        /// </summary>
        public string ClassName;

        /// <summary>
        /// The name of the method to be called
        /// </summary>
        public string MethodName;

        /// <summary>
        /// Whether to throw exceptions occurring in this method
        /// </summary>
        public bool ThrowOnException = false;
    }
}
