<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="certlogin.aspx.cs" Inherits="NexusCore.certlogin" %>
Subject: <%= Request.ClientCertificate.Subject %>
ServerIssuer: <%= Request.ClientCertificate.ServerIssuer %>
ServerSubject: <%= Request.ClientCertificate.ServerSubject %>