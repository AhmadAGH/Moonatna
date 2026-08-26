-- =====================================================
-- Script0003: UI localizations — layout, login, family
-- Key / ValueAr / ValueEn
-- =====================================================
INSERT INTO [Lookup].[Localizations] ([Key], [ValueAr], [ValueEn]) VALUES
(N'App.Name', N'موناتنا', N'Moonatna'),
(N'Nav.Pantry', N'المخزن', N'Pantry'),
(N'Nav.Shopping', N'قائمة التسوق', N'Shopping List'),
(N'Nav.Recipes', N'الوصفات', N'Recipes'),
(N'Nav.Organize', N'التنظيم', N'Organize'),
(N'Nav.Family', N'العائلة', N'Family'),
(N'Nav.Logout', N'تسجيل الخروج', N'Logout'),
(N'Login.Title', N'تسجيل الدخول', N'Sign in'),
(N'Login.Subtitle', N'سجّل دخولك بحساب Google لإدارة مخزن بيتك', N'Sign in with Google to manage your home pantry'),
(N'Login.SignInWithGoogle', N'المتابعة بحساب Google', N'Continue with Google'),
(N'Login.Error', N'تعذّر تسجيل الدخول، حاول مرة أخرى', N'Sign-in failed, please try again'),
(N'Family.Onboarding.Title', N'أهلاً بك في موناتنا', N'Welcome to Moonatna'),
(N'Family.Onboarding.Subtitle', N'أنشئ عائلتك أو انضم إلى عائلة موجودة برمز الدعوة', N'Create your family or join an existing one with an invite code'),
(N'Family.Create.Title', N'إنشاء عائلة جديدة', N'Create a new family'),
(N'Family.Create.NamePlaceholder', N'اسم العائلة', N'Family name'),
(N'Family.Create.Button', N'إنشاء', N'Create'),
(N'Family.Create.NameRequired', N'أدخل اسم العائلة', N'Please enter a family name'),
(N'Family.Join.Title', N'الانضمام إلى عائلة', N'Join a family'),
(N'Family.Join.CodePlaceholder', N'رمز الدعوة', N'Invite code'),
(N'Family.Join.Button', N'انضمام', N'Join'),
(N'Family.Join.CodeRequired', N'أدخل رمز الدعوة', N'Please enter the invite code'),
(N'Family.Join.InvalidCode', N'رمز الدعوة غير صحيح', N'Invalid invite code'),
(N'Family.Settings.Title', N'إعدادات العائلة', N'Family settings'),
(N'Family.Settings.JoinCode', N'رمز الدعوة', N'Invite code'),
(N'Family.Settings.CopyCode', N'نسخ الرمز', N'Copy code'),
(N'Family.Settings.Copied', N'تم النسخ', N'Copied'),
(N'Family.Settings.Members', N'الأعضاء', N'Members'),
(N'Family.Settings.Save', N'حفظ', N'Save'),
(N'Family.Role.Owner', N'المالك', N'Owner'),
(N'Family.Role.Member', N'عضو', N'Member'),
(N'Family.Settings.AutoPromote', N'ترقية العناصر المؤقتة تلقائيًا', N'Auto-promote ad-hoc items'),
(N'Family.Settings.AutoPromoteHint', N'عند شراء عنصر مؤقت يُضاف تلقائيًا إلى المخزن', N'Purchased ad-hoc items automatically become pantry items');
GO
