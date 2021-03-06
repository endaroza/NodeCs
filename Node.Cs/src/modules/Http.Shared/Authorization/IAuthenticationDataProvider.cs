// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


namespace Http.Shared.Authorization
{
	public interface IUser
	{
		int UserId { get; }
		string UserName { get; }
		bool ChangePassword(string oldPassword, string newPassword);
		void SetRoles(params string[] roleNames);
	}
	public class NullAuthenticationDataProvider :  IAuthenticationDataProvider
	{
		static NullAuthenticationDataProvider _instance = new NullAuthenticationDataProvider();
		public static IAuthenticationDataProvider Instance
		{
			get
			{
				return _instance;
			}
		}
		private NullAuthenticationDataProvider()
		{

		}
		public bool IsUserAuthorized(string user, string password)
		{
			return false;
		}

		public IUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out AuthenticationCreateStatus status)
		{
			status = AuthenticationCreateStatus.ProviderError;
			return null;
		}

		public bool IsUserPresent(string userName)
		{
			return false;
		}

		public IUser GetUser(string userName, bool isOnLine)
		{
			return null;
		}

		public void AddUsersToRoles(string[] userNames, string[] roleNames)
		{

		}

		public string[] GetUserRoles(string p)
		{
			return new string[0];
		}

		public bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			return true;
		}
	}
	public interface IAuthenticationDataProviderFactory
	{
		IAuthenticationDataProvider CreateAuthenticationDataProvider();
	}
	public interface IAuthenticationDataProvider
	{
		bool IsUserAuthorized(string user, string password);
		IUser CreateUser(string username, string password, string email, string passwordQuestion,
		string passwordAnswer, bool isApproved, object providerUserKey, out AuthenticationCreateStatus status);
		bool IsUserPresent(string userName);
		IUser GetUser(string userName, bool isOnLine);
		void AddUsersToRoles(string[] userNames, string[] roleNames);
		string[] GetUserRoles(string p);
		bool DeleteUser(string username, bool deleteAllRelatedData);
	}

	public enum AuthenticationCreateStatus
	{
		Success,
		DuplicateUserName,
		DuplicateEmail,
		InvalidPassword,
		InvalidAnswer,
		InvalidEmail,
		InvalidQuestion,
		InvalidUserName,
		ProviderError,
		UserRejected
	}
}
