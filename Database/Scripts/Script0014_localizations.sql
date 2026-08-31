UPDATE [Lookup].[Localizations]
SET [ValueAr] = N'سجّل دخولك لإدارة مخزن بيتك وقائمة تسوق عائلتك',
    [ValueEn] = N'Sign in to manage your home pantry and family shopping list'
WHERE [Key] = N'Login.Subtitle';

INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'App.Tagline', N'مؤونة بيتك.. في مكان واحد', N'Your home pantry, all in one place'),
(N'Login.Email', N'البريد الإلكتروني', N'Email'),
(N'Login.Password', N'كلمة المرور', N'Password'),
(N'Login.SubmitButton', N'تسجيل الدخول', N'Sign in'),
(N'Login.ForgotPassword', N'نسيت كلمة المرور؟', N'Forgot password?'),
(N'Login.ResetSent', N'تم إرسال رابط إعادة تعيين كلمة المرور إلى بريدك الإلكتروني', N'A password reset link has been sent to your email'),
(N'Login.OrDivider', N'أو سجّل الدخول عبر', N'Or sign in via'),
(N'Login.NoAccount', N'أول مرة في مونتتا؟', N'First time in Moonatna?'),
(N'Login.CreateAccount', N'أنشئ بيتك الآن', N'Create your household'),
(N'Register.Title', N'أنشئ بيتك الآن', N'Create your household'),
(N'Register.Subtitle', N'أنشئ حسابك لتبدأ بتنظيم مؤونة بيتك', N'Create your account to start organizing your home pantry'),
(N'Register.DisplayName', N'اسمك', N'Your name'),
(N'Register.ConfirmPassword', N'تأكيد كلمة المرور', N'Confirm password'),
(N'Register.Submit', N'إنشاء الحساب', N'Create account'),
(N'Register.HaveAccount', N'لديك حساب بالفعل؟', N'Already have an account?'),
(N'Register.SignIn', N'سجّل الدخول', N'Sign in'),
(N'Register.PasswordMismatch', N'كلمتا المرور غير متطابقتين', N'Passwords do not match'),
(N'Register.Error', N'تعذّر إنشاء الحساب، حاول مرة أخرى', N'Could not create your account, please try again');
GO
