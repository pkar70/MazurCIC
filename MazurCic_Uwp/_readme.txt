
Migracja z Uno - zabrałem stamtąd.

C#, żeby po prostu przenieść kod z Uno, bez żadnych zabaw, ale mieć wersję na telefon.
Uno zaś zostanie tylko na Android, PWA, iOS. 

Bo:
	Uno 4.7.37
	Xamarin max android 13, ale gogusstore min android 14 - konieczna migracja do przynajmniej Uno 5.
	Kolejne Uno, Uno5 - wyrzuca Xamarin, zostaje UWP oraz WinUI. Można byłoby go użyć, zostając na UWP, ale nie da się prostej migracji zrobić (nie wystarczy nuget update, bo się rzuca że nie obsługuje Xamarin, a add-project Uno wymaga teraz single project)
	Kolejne Uno, Uno6 - wyrzuca UWP, już tylko WinUI. Ale WinUI z kolei nie działa na telefonie.

