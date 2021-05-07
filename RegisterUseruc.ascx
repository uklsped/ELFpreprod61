<%@ Control Language="VB"  AutoEventWireup="false" CodeFile="RegisterUseruc.ascx.vb" Inherits="RegisterUseruc" enableviewstate="false"%>

    <script type="text/javascript"> 
//This function prevents return key usage from http://www.felgall.com/jstip43.htm
        function kH(e) {
           var pK = e ? e.which : window.event.keyCode;
            return pK != 13;>
        }
        document.onkeypress = kH;
        if (document.layers) document.captureEvents(Event.KEYPRESS);
        function Reset1_onclick() {

        }

    </script>
<%--<link href="App_Themes/Blue/Elf.css" rel="stylesheet" type="text/css" />--%>
<%--<link rel="stylesheet" type="text/css" href="css\twocol.css" />--%>
<style type="text/css">
    .style1
    {
        height: 23px;
    }
    .style2
    {
        height: 28px;
    }
</style>
<%--<asp:UpdatePanel ID="UpdatePanel1" runat="server">
<ContentTemplate>--%>
<div id="container" style="background-color: #66FFFF; float: none; padding: 0px; margin: 0px auto 0px auto; overflow: hidden;">
    
    <div id="leftcoll" style="float: left; width: auto;">
    <fieldset>
            <legend>Register</legend>
            
            <asp:CreateUserWizard ID="CreateUserWizard1" runat="server" 
                disablecreateduser="True" 
     
     LoginCreatedUser="False" EnableViewState="False" 
                oncreateusererror="OnCreateUserError" style="margin-right: 114px" 
                Width="391px" >
        <WizardSteps>
            <asp:CreateUserWizardStep ID="CreateUserWizardStep1" runat="server">
                <ContentTemplate>
                    <table border="0">
                        <tr>
                            <td align="center" colspan="2" class="style1">
                                </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="FirstNameLabel" runat="server" AssociatedControlID="FirstName">First Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="FirstName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="FirstNameRequired" runat="server" 
                                    ControlToValidate="FirstName" ErrorMessage="First Name is required." 
                                    ToolTip="First Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="LastNameLabel" runat="server" AssociatedControlID="LastName">Last Name:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="LastName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="LastNameRequired" runat="server" 
                                    ControlToValidate="LastName" ErrorMessage="Last Name is required." 
                                    ToolTip="Last Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                            </td>
                            <td>
                                
                                <asp:TextBox ID="UserName" runat="server" ViewStateMode="Disabled"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                    ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                    ToolTip="User Name is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="Password">Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Password" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" 
                                    ControlToValidate="Password" ErrorMessage="Password is required." 
                                    ToolTip="Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="ConfirmPasswordLabel" runat="server" 
                                    AssociatedControlID="ConfirmPassword">Confirm Password:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="ConfirmPassword" runat="server" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="ConfirmPasswordRequired" runat="server" 
                                    ControlToValidate="ConfirmPassword" 
                                    ErrorMessage="Confirm Password is required." 
                                    ToolTip="Confirm Password is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="EmailLabel" runat="server" AssociatedControlID="Email">E-mail:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="Email" runat="server"></asp:TextBox>
                                
                            </td>
                            <td>
                            <asp:RegularExpressionValidator ID="EmailValidator" runat="server" 
                                    ControlToValidate="Email" ValidationExpression="^\S+@\S+\.\S+$" ErrorMessage="You must enter a valid E-mail address."
                                    Display="Dynamic"
                                    ToolTip="E-mail is required." ValidationGroup="CreateUserWizard1"/>
                            </td>
                        </tr>
                        <tr>
                            <td align="right">
                                <asp:Label ID="RoleLabel" runat="server" AssociatedControlID="UserRole">User Role:</asp:Label>
                            </td>
                            <td>
                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" 
                                    ConnectionString="<%$ ConnectionStrings:userstring %>" 
                                    SelectCommand="SELECT [RoleName] FROM [aspnet_Roles] where [Description] != 1"></asp:SqlDataSource>
                                <asp:DropDownList ID="UserRole"  runat="server"  Width="128px" 
                                    DataSourceID="SqlDataSource1" DataTextField="RoleName" 
                                    DataValueField="RoleName">
                </asp:DropDownList>
                                <%--<asp:TextBox ID="UserRole" runat="server"></asp:TextBox>--%>
                                <asp:RequiredFieldValidator ID="RoleRequired" runat="server" 
                                    ControlToValidate="UserRole" ErrorMessage="User Role is required." 
                                    ToolTip="User Role is required." ValidationGroup="CreateUserWizard1">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        
                        <tr>
                            <td align="center" colspan="2">
                                <asp:CompareValidator ID="PasswordCompare" runat="server" 
                                    ControlToCompare="Password" ControlToValidate="ConfirmPassword" 
                                    Display="Dynamic" 
                                    ErrorMessage="The Password and Confirmation Password must match." 
                                    ValidationGroup="CreateUserWizard1"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <td align="center" colspan="2" style="color:Red;">
                                <asp:Literal ID="ErrorMessage" runat="server" EnableViewState="False"></asp:Literal>
                            </td>
                        </tr>
                    </table>
                    <asp:Label id="Label1" 
        runat="server">
      </asp:Label>
                    <asp:SqlDataSource ID="InsertExtraInfo" runat="server" ConnectionString="<%$ ConnectionStrings:userstring %>"
                    InsertCommand="Insert into [FirstLastName] (UserID, FirstName, LastName) Values (@UserID, @FirstName, @LastName)"
                    ProviderName="<%$ ConnectionStrings:userstring.ProviderName %>">
                    <InsertParameters>
                    <asp:ControlParameter Name="FirstName" Type="String" ControlID="FirstName" PropertyName="Text" />
                    <asp:ControlParameter Name="LastName" Type="String" ControlID="LastName" PropertyName="Text" />
                    </InsertParameters>
                    </asp:SqlDataSource>
                    <table border="0" style="font-size: 100%; font-family: Verdana" id="TABLE1" >
            
        </table>
                    
                    
                </ContentTemplate>
               <CustomNavigationTemplate>
                    <table border="0" cellspacing="5" style="width:100%;height:100%;">
                        <tr align="right">
                            <td align="right" colspan="0">
                                <asp:Button ID="StepNextButton" runat="server" CommandName="MoveNext" 
                                    Text="Create User" ValidationGroup="CreateUserWizard1" />
                            </td>
                             <td align="right">
                                 <input id="Reset2" type="reset" value="Clear" />
                             <%--<asp:Button ID="Button3" runat="server" Text="Cancel" OnClick="ClearPage" />--%>
                                  </td>
                            
                        </tr>
                    </table>
                </CustomNavigationTemplate>
            </asp:CreateUserWizardStep>
            <asp:CompleteWizardStep ID="CompleteWizardStep1" runat="server">

            <ContentTemplate>
        <table border="0" style="font-size: 100%; font-family: Verdana" id="TABLE1" >
            <tr>
                <td align="center" colspan="2" style="font-weight: bold; color: white; background-color: #5d7b9d; height: 18px;">
                    Complete</td>
            </tr>
            <tr>
                
                <td>
                    <p>Your account has been successfully created. You will be e-mailed when it has been approved.</p>.<br />
                    <br />
                   
            </tr>
            <tr>
                <td align="right" colspan="2">
                    &nbsp;<asp:Button ID="ContinueButton" runat="server" BackColor="#FFFBFF" BorderColor="#CCCCCC"
                        BorderStyle="Solid" BorderWidth="1px" CausesValidation="False" Font-Names="Verdana" ForeColor="#284775" Text="EXIT" ValidationGroup="CreateUserWizard1" OnClick="ClearPage"/>
                </td>
            </tr>
        </table>
    </ContentTemplate>
            </asp:CompleteWizardStep>
        </WizardSteps>
    </asp:CreateUserWizard>
            </fieldset>
    </div>
    <div id="centrecoll" style="float: left; width: auto">
    <fieldset style="width:auto;">
    <legend>Recover Password</legend>
        
       <asp:PasswordRecovery ID="PasswordRecovery1" runat="server" EnableViewState="false" ViewStateMode="Disabled">
            <UserNameTemplate>
                <table cellpadding="1" cellspacing="0" style="border-collapse:collapse;">
                    <tr>
                        <td>
                            <table cellpadding="0">
                                <tr>
                                    <td align="center" colspan="2">
                                        Forgotten Your Password?</td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2">
                                       <p> Enter your User Name to be e-mailed a temporary password.</p></td>
                                </tr>
                                <tr>
                                    <td align="right" class="style2">
                                        <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName" Text="">User Name:</asp:Label>
                                    </td>
                                    <td class="style2">
                                        <asp:TextBox ID="UserName" runat="server" CausesValidation="True" 
                                            EnableViewState="False"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                            ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                            ToolTip="User Name is required." ValidationGroup="ctl00$PasswordRecovery1">*</asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="2" style="color:Red;">
                                        <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" colspan="1">
                                        <asp:Button ID="SubmitButton" runat="server" CommandName="Submit" Text="Recover Password" ValidationGroup="ctl00$PasswordRecovery1"  />
                                            <td align="center">
                                                <asp:Button id="Reset3" causesvalidation="false" Text="Clear" OnClick="ClearPage" runat="server" />
                                                
                                            </td>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </UserNameTemplate>
</asp:PasswordRecovery>
</fieldset>
       <fieldset style="width:auto;">
    <legend>Change Password</legend>
    
           <asp:ChangePassword ID="ChangePwd" runat="server" 
        DisplayUserName="true" DisplayCancelButton="True" DisplayContinueButton="false"
            >
               <ChangePasswordTemplate>
                   <table cellpadding="1" cellspacing="0" style="border-collapse:collapse;">
                       <tr>
                           <td>
                               <table cellpadding="0">
                                   <tr>
                                       <td align="center" colspan="2">
                                           Change Your Password</td>
                                   </tr>
                                   <tr>
                                       <td align="right" class="style2">
                                           <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="UserName">User Name:</asp:Label>
                                       </td>
                                       <td class="style2">
                                           <asp:TextBox ID="UserName" runat="server" EnableViewState="false" ViewStateMode="Disabled" ></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" 
                                               ControlToValidate="UserName" ErrorMessage="User Name is required." 
                                               ToolTip="User Name is required." ValidationGroup="ctl00$ChangePwd">*</asp:RequiredFieldValidator>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="right">
                                           <asp:Label ID="CurrentPasswordLabel" runat="server" 
                                               AssociatedControlID="CurrentPassword">Password:</asp:Label>
                                       </td>
                                       <td>
                                           <asp:TextBox ID="CurrentPassword" runat="server" TextMode="Password"></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="CurrentPasswordRequired" runat="server" 
                                               ControlToValidate="CurrentPassword" ErrorMessage="Password is required." 
                                               ToolTip="Password is required." ValidationGroup="ctl00$ChangePwd">*</asp:RequiredFieldValidator>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="right">
                                           <asp:Label ID="NewPasswordLabel" runat="server" 
                                               AssociatedControlID="NewPassword">New Password:</asp:Label>
                                       </td>
                                       <td>
                                           <asp:TextBox ID="NewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="NewPasswordRequired" runat="server" 
                                               ControlToValidate="NewPassword" ErrorMessage="New Password is required." 
                                               ToolTip="New Password is required." ValidationGroup="ctl00$ChangePwd">*</asp:RequiredFieldValidator>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="right">
                                           <asp:Label ID="ConfirmNewPasswordLabel" runat="server" 
                                               AssociatedControlID="ConfirmNewPassword">Confirm New Password:</asp:Label>
                                       </td>
                                       <td>
                                           <asp:TextBox ID="ConfirmNewPassword" runat="server" TextMode="Password"></asp:TextBox>
                                           <asp:RequiredFieldValidator ID="ConfirmNewPasswordRequired" runat="server" 
                                               ControlToValidate="ConfirmNewPassword" 
                                               ErrorMessage="Confirm New Password is required." 
                                               ToolTip="Confirm New Password is required." ValidationGroup="ctl00$ChangePwd">*</asp:RequiredFieldValidator>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="center" colspan="2">
                                           <asp:CompareValidator ID="NewPasswordCompare" runat="server" 
                                               ControlToCompare="NewPassword" ControlToValidate="ConfirmNewPassword" 
                                               Display="Dynamic" 
                                               ErrorMessage="The Confirm New Password must match the New Password entry." 
                                               ValidationGroup="ctl00$ChangePwd"></asp:CompareValidator>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="center" colspan="2" style="color:Red;">
                                           <asp:Literal ID="FailureText" runat="server" EnableViewState="False"></asp:Literal>
                                       </td>
                                   </tr>
                                   <tr>
                                       <td align="right">
                                           <asp:Button ID="ChangePasswordPushButton" runat="server" 
                                               CommandName="ChangePassword" Text="Change Password" 
                                               ValidationGroup="ctl00$ChangePwd" 
                                                />
                                       </td>
                                       <td align="right">
                                           <input id="Reset1" type="reset" value="Clear"/>
                                            <%--<asp:Button ID="Button3" runat="server" Text="Cancel" OnClick="ClearPage" causesvalidation="false"/>--%>
                                       </td>
                                   </tr>
                               </table>
                           </td>
                       </tr>
                   </table>
               </ChangePasswordTemplate>

               <SuccessTemplate>
                   <table cellpadding="1" cellspacing="0" style="border-collapse:collapse;">
                       <tr>
                           <td>
                               <table cellpadding="0">
                                   <tr>
                                       <td align="center">
                                           Change Password Complete</td>
                                   </tr>
                                   <tr>
                                       <td>
                                           Your password has been changed!</td>
                                   </tr>
                                   <tr>
                                       <td align="right">
                                         <asp:Button ID="ContinuePushButton" Text="Close" runat="server" CausesValidation="False" 
                                               OnClick="ClearPage" />
                                       </td>
                                   </tr>
                               </table>
                           </td>
                       </tr>
                   </table>
               </SuccessTemplate>

    </asp:ChangePassword>
   
    </fieldset>
       
        
    </div>
    
<div id="rightcoll">
    <asp:Button ID="Button4" runat="server" Text="Exit" CausesValidation="false"  OnClick="ClearPage" Width="200px" Height="100px"/>
   
<br />
<br />
 <div>  
       <%-- <h2>DeleteUser method example</h2>--%>  
        <asp:Label ID="Label1" runat="server" Font-Bold="true" ForeColor="DarkCyan"></asp:Label>  
       <%-- <br /><br />  
        <b>Select user</b>  
        <br />  
        <asp:DropDownList ID="DropDownList1" runat="server" BackColor="AliceBlue"></asp:DropDownList>  
        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="DropDownList1" Text="*"></asp:RequiredFieldValidator>  
        <br /><br />  --%>
        <%--<asp:Button ID="Button1" runat="server" Text="Delete selected user" OnClick="Button1_Click" /> --%> 
    </div>
    <asp:HiddenField ID="HiddenField1" runat="server" />
</div>
</div>
<%--</ContentTemplate>
<Triggers>
        <asp:PostBackTrigger ControlID="Button4" />
    </Triggers>
</asp:UpdatePanel>--%>




