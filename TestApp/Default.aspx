<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="TestApp._Default" %>
<%@ Import Namespace="stillbreathing.co.uk.ActionManager" %><!DOCTYPE html>

<html>
<head>
    <title>ActionManager: Custom actions for your ASP.NET web applications</title>
</head>
<body>

    <h1>ActionManager</h1>
    
    <h2>Introduction</h2>
    
    <p>ActionManager is an open-source system to run custom code at defined points in your .NET applications, without know where (or even if) those points occur. ActionManager is modelled on <a href="http://codex.wordpress.org/Plugin_API">WordPress' Plugin API</a>.</p>
    
    <p>ActionManager runs actions called, amazingly, Actions.  To 'hook' your method into an Action an ActionMethod (after first adding a reference to the stillbreathing.co.uk.ActionManager DLL, and optionally add a using statement at the top of your page). Each ActionMethod can be created with a single line of code:</p>
    
    <p>Each ActionMethod can be created with a single line of code:</p>
    
    <code>ActionMethod MyMethod = new ActionMethod("Namespace.Class.Method");</code>
    
    <p>There are two overloads for the ActionMethod class. The first accepts a boolean parameter to set whether the target class/method is static (and therefore does not need to be instantiated):</p>
    
    <code>ActionMethod MyMethod = new ActionMethod("Namespace.Class.Method", true);</code>
    
    <p>The second overload adds another boolean parameter to set whether the target method accepts an object parameter. The object parameter can be modified and returned to the calling Action:</p>
    
    <code>ActionMethod MyMethod = new ActionMethod("Namespace.Class.Method", true, true);</code>

    <p>Then add the ActionMethod to the Action using the ActionManager.AddMethod() method. This call to ActionManager.AddMethod() means that whenever the 'MyAction' action occurs, the ActionMethod defined in 'MyMethod' will be executed:</p>

    <code>ActionManager.AddMethod("MyAction", MyMethod);</code>
    
    <p>You can also set a priority for the ActionMethod; higher numbers mean the ActionMethod will run before other ActionMethods for this Action with a lower priority:</p>
    
    <code>ActionManager.AddMethod("MyAction", MyMethod, 1000);</code>
    
    <h2>ActionMethod properties</h2>
    
    <p>The following properties can be set for a ActionMethod:</p>
    
    <ul>
        <li><strong>ActionName</strong> (string): The name of the action this method will be called for</li>
        <li><strong>Priority</strong> (int, default 0): The priority of this method, higher numbers are executed first</li>
        <li><strong>IsStatic</strong> (boolean, default true): Whether this method is static, or needs instantiating</li>
        <li><strong>AssemblyName</strong> (string): The name of the assembly containing this method</li>
        <li><strong>Namespace</strong> (string): The namespace of this class</li>
        <li><strong>ClassName</strong> (string): The name of the class to be called</li>
        <li><strong>MethodName</strong> (string): The name of the method to be called</li>
        <li><strong>AcceptsParameters</strong> (boolean, default false): Whether this method accepts parameters and returns an object</li>
        <li><strong>ThrowOnException</strong> (boolean, default false): Whether to throw exceptions occurring in this method</li>
    </ul>
    
    <h2>ActionMethod Tests</h2>
    
    <h3>Standard Actions</h3>

    <% ActionManager.PerformAction("TestAction"); %>
    
    <h3>Actions with parameters</h3>
    
    <% ActionManager.PerformAction("TestActionWithObject", "Object 1"); %>
    
    <h3>Method test</h3>
    
    <p>For each row in this table a method is called that sets the content of the 'Extra' column.</p>
    
    <asp:Repeater id="PeopleTable" runat="server">
    <HeaderTemplate>
    <table>
    <tr>
        <th>ID</th>
        <th>Name</th>
        <th>Age</th>
        <th>Extra</th>
    </tr>
    </HeaderTemplate>
    <ItemTemplate>
    <tr>
        <td><%# Eval("id") %></td>
        <td><%# Eval("name") %></td>
        <td><%# Eval("age") %></td>
        <td><%# Eval("extra") %></td>
    </tr>
    </ItemTemplate>
    <FooterTemplate>
    </table>
    </FooterTemplate>
    </asp:Repeater>
    
</body>
</html>
