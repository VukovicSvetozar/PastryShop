# Uvod
&nbsp;&nbsp;&nbsp;&nbsp;***PastryShop*** je desktop WPF aplikacija osmišljena da pojednostavi i unaprijedi svakodnevno poslovanje malih i srednjih pekara, poslastičarnica i srodnih objekata sa brzim prometom robe.  
Cilj ovog korisničkog uputstva je da na jasan i praktičan način predstavi početna podešavanja, svakodnevne operacije, kao i napredne funkcije aplikacije, omogućavajući brzo uvođenje u rad i korištenje svih njenih prednosti.

## Pregled ključnih funkcionalnosti  
&nbsp;&nbsp;&nbsp;&nbsp;U nastavku su opisane osnovne funkcionalnosti aplikacije.

**Upravljanje korisničkim nalozima**  
&nbsp;&nbsp;&nbsp;&nbsp;Jasno definisane uloge i kontrole pristupa doprinose većoj sigurnosti i pouzdanosti rada aplikacije.

**Centralizovano upravljanje proizvodima i zalihama**  
&nbsp;&nbsp;&nbsp;&nbsp;Sistem podržava upravljanje proizvodima i zalihama, omogućavajući pregled, dodavanje i uređivanje artikala, praćenje količina, serija nabavki i rokova trajanja, čime se olakšava planiranje i kontrola poslovanja.

**Brz i intuitivan POS**  
&nbsp;&nbsp;&nbsp;&nbsp;Jednostavno kreiranje porudžbina, rad sa artiklima u korpi i brza naplata gotovinom, karticom ili odloženo plaćanje, uz mogućnost poništavanja ranije kreiranih porudžbina.

**Personalizacija**  
&nbsp;&nbsp;&nbsp;&nbsp;Sistem podržava više jezika, prilagodljive vizuelne teme i izmjenu profila korisnika, omogućavajući bolje podešavanje interfejsa prema individualnim potrebama.

**Izvještaji i statistika**  
&nbsp;&nbsp;&nbsp;&nbsp;Omogućen je brz i pregledan uvid u prodaju, zalihe i ključne poslovne pokazatelje kroz intuitivne panele i grafike.

# Uloge i autentifikacija  
&nbsp;&nbsp;&nbsp;&nbsp;*PastryShop* unapređuje efikasnost i sigurnost poslovanja kroz jasno definisane korisničke uloge. Ovakav pristup omogućava preciznu podjelu odgovornosti i sprječava neovlašćen pristup važnim funkcijama sistema, čime se smanjuje rizik od grešaka i mogućih zloupotreba.

## Uloge u sistemu  
&nbsp;&nbsp;&nbsp;&nbsp;Aplikacija razlikuje dvije osnovne uloge korisnika:

**Menadžer**
-   Posjeduje administrativnu kontrolu nad cijelim sistemom.
-   Odgovoran je za kreiranje i upravljanje nalozima korisnika (dodavanje novih i deaktivacija postojećih naloga, resetovanje lozinki).
-   Vrši nadzor nad proizvodima i zalihama.
-   Ima pristup izvještajima i statistikama radi praćenja i analize poslovanja.

**Blagajnik**
-   Koristi POS interfejs prilagođen za brz i efikasan rad na prodajnom mjestu.
-   Njegove primarne funkcije obuhvataju kreiranje porudžbina, naplatu i pregled dnevnih transakcija.

## Proces prijave i autentifikacija  
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer ima mogućnost da kreira korisničke naloge za zaposlene, dodjeljuje im odgovarajuće uloge i generiše početnu lozinku.  
Zaposleni dobijaju lozinku i preporuka je da je, radi sigurnosti, promijene pri prvoj prijavi. Korisnici se prijavljuju putem svojih jedinstvenih
kredencijala. Po pokretanju aplikacije otvara se ekran za prijavu sa poljima za unos ***korisničkog imena*** i ***lozinke***. Navigacija između polja omogućena je tasterom *Tab*. Polja su obavezna, a dozvoljen je jedino unos slova i cifara. Definisan je minimalni i maksimalni broj unesenih karaktera.

&nbsp;&nbsp;&nbsp;&nbsp;Korisnicima je na raspolaganju ***opcija promjene jezika***, pri čemu se odabrani jezik primjenjuje samo na stranicu za prijavu. Kao prečica može da se koristi kombinacija tastera *Ctrl + L* kojom se otvara padajući meni za izbor jezika.  

&nbsp;&nbsp;&nbsp;&nbsp;Nakon klika na dugme ***„Prijava"*** *(Login)* ili pritiskom na taster *Enter*, sistem provjerava kredencijale i u slučaju uspjeha: evidentira vrijeme prijave i preusmjerava korisnika na početni ekran u skladu s njegovom ulogom.  
Menadžeri se preusmjeravaju na kontrolnu tablu za rad sa korisničkim nalozima.  
Blagajnici odmah pristupaju POS ekranu optimizovanom za prodaju i naplatu.  
Ovakav pristup omogućava da svaki korisnik radi u okruženju precizno prilagođenom njegovim zadacima.  

&nbsp;&nbsp;&nbsp;&nbsp;U slučaju da prijava nije uspješna, pojavljuje se dijalog upozorenja sa informacijom o razlogu neuspjele operacije.  
Razlozi mogu biti: netačno korisničko ime ili lozinka; nalog nije aktivan; nekorektne vrijednosti polja i tada su uz data polja prikazane odgovarajuće poruke radi lakše korekcije pogrešnih unosa.

&nbsp;&nbsp;&nbsp;&nbsp;Na ekranu se nalazi i dugme ***„Otkaži"*** *(Cancel)* za izlaz iz aplikacije. Za ovu funkcionalnost omogućena je prečica *Escape (Esc)*.

# Menadžer

&nbsp;&nbsp;&nbsp;&nbsp;Uloga menadžera je da osigura stabilnost i urednost poslovanja, kako bi svakodnevni rad u pekarskom ili poslastičarskom objektu tekao nesmetano.

**Glavne odgovornosti menadžera su:**
-   upravljanje korisnicima,
-   upravljanje proizvodima,
-   kontrola zaliha,
-   kreiranje izvještaja.

## Prijava i pristup
&nbsp;&nbsp;&nbsp;&nbsp;Po uspješnoj prijavi na sistem prikazuje se početni ekran za menadžere.  

&nbsp;&nbsp;&nbsp;&nbsp;Sa lijeve strane ekrana nalazi se vertikalni ***navigacioni meni*** sa ikonama i nazivima opcija. Ovaj meni omogućava brz pristup ključnim modulima i funkcijama sistema, a svaka opcija ima i odgovarajuću tastaturnu prečicu za dodatnu efikasnost:  
-   **Zaposleni** - pregled i upravljanje korisničkim nalozima (*Ctrl + U*).
-   **Proizvodi** - katalogizacija i uređivanje proizvoda (*Ctrl + P*).
-   **Izvještaj** - pregled i generisanje prodajnih i inventarskih izvještaja (*Ctrl + R*).
-   **Postavke** - podešavanje jezika i teme aplikacije (*Ctrl + S*).
-   **Profil** - uređivanje profila korisnika (*Ctrl + M*).
-   **Odjava** - izlazak iz aplikacije (*Esc*).

&nbsp;&nbsp;&nbsp;&nbsp;Sa desne strane ekrana nalazi se glavni dio, u kojem se prikazuje ***sadržaj izabranog modula***. Ovdje menadžer unosi podatke i ima pregled svih relevantnih informacija o poslovanju, kao što su evidencija zaposlenih, stanje zaliha, podaci o proizvodima i izvještaji.

## Upravljanje korisnicima

### Pregled korisnika
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer ima pristup svim korisnicima aplikacije, što omogućava jednostavno praćenje i uređivanje njihovih naloga.  
Dostupna je tabela koja prikazuje ***osnovne informacije*** o svakom korisniku: *ID, korisničko ime, tip korisnika* (*Menadžer* ili *Blagajnik*), kao i dostupne akcije kroz ikone za *detaljan pregled naloga, promjenu statusa, promjenu lozinke* i slične radnje.

&nbsp;&nbsp;&nbsp;&nbsp;***Polje za pretragu*** omogućava brzo pronalaženje korisnika (*Tab* ili *F3* prebacuje fokus na polje), dok padajući meni omogućava ***filtriranje po statusu*** (*Ctrl + F*).  

&nbsp;&nbsp;&nbsp;&nbsp;Korisnici koji imaju status *„neaktivan"* jasno su istaknuti u prikazu.

### Dodavanje korisnika
&nbsp;&nbsp;&nbsp;&nbsp;Dugme ***Dodaj*** u gornjem desnom uglu glavnog ekrana otvara prozor sa formularom za kreiranje novog korisničkog naloga (*Ctrl + N*).

&nbsp;&nbsp;&nbsp;&nbsp;Formular sadrži *osnovna, zajednička polja* koja se popunjavaju za sve korisnike, dok se *specifična polja* pojavljuju u zavisnosti od izabranog tipa korisnika.

**Zajednička polja**

-   *Tip korisnika* - padajući meni gdje je neophodno odabrati: *Manager* ili *Cashier*,
-   *Korisničko ime* - obavezno tekstualno polje koje provjerava jedinstvenost imena, dozvoljava samo važeće znakove uz minimalnu i maksimalnu dužinu,
-   *Lozinka* - obavezno tekstualno polje (maskirano) sa sličnim ograničenjima kao polje za unos korisničkog imena,
-   *Ime* - tekstualno polje koje dozvoljava jedino unos slova, brojeva i razmaka,
-   *Prezime* - ista funkcionalnost i ograničenje kao polje za unos imena,
-   *Broj telefona* - tekstualno polje koje prihvata samo ispravan format telefonskog broja,
-   *Adresa* - slobodno tekstualno polje za unos adrese,
-   *Plata* - obavezno numeričko polje koje ne može biti negativno,
-   *Putanja do slike (Izaberi sliku)* - dugme za odabir fajla; nakon odabira prikazuje se preview slike. Dozvoljeni formati: *.jpg, jpeg, .png*.

**Specijalna polja - Menadžer**  

&nbsp;&nbsp;&nbsp;&nbsp;Prikazuju se samo kada je izabran Manager:

-   *Odjeljenje* - tekstualno polje koje dozvoljava samo unos slova, brojeva i razmake.

**Specijalna polja - Blagajnik**  

&nbsp;&nbsp;&nbsp;&nbsp;Prikazuju se samo kada je izabran Cashier:

-   *ID kase* - numeričko polje koje prihvata samo cijele brojeve,
-   *Početak smjene* - omogućava prikaz kalendara za odabir datum i vremena; ne može biti u prošlosti,
-   *Kraj smjene* - odabrani datum + vrijeme; mora biti poslije početka smjene i trajanje smjene mora biti najmanje jedan sat.

&nbsp;&nbsp;&nbsp;&nbsp;Provjerava se ispravnost unesenih podataka i ukoliko su svi podaci validni, sprema novi korisnički nalog.  
U suprotnom se prikazuje odgovarajući dijalog upozorenja. Takođe, poruke o greškama prikazuju se odmah pored nevalidnih polja radi lakše korekcije.

### Prikaz detalja korisnika  
&nbsp;&nbsp;&nbsp;&nbsp;Za selektovanog korisnika, klikom na odgovarajuću ikonicu otvara se prozor sa ***detaljnim informacijama*** o tom korisniku.  
Prozor prikazuje osnovne podatke, uključujući *profilnu sliku, tip korisnika, korisničko ime, ime i prezime, broj telefona i adresu, datum zaposlenja, platu i posljednju prijavu u sistem*, kao i dodatne podatke prema tipu korisnika: *odjeljenje* za menadžera ili *ID kase* i *smjene* za blagajnika.

### Uređivanje korisnika  
&nbsp;&nbsp;&nbsp;&nbsp;Klikom na odgovarajuću ikonicu za odabranog korisnika otvara se prozor za ***uređivanje osnovnih podataka*** vezanih za poslovanje. U ovom prozoru moguće je mijenjati *platu* zaposlenog, a u zavisnosti od tipa korisnika, dozvoljena je izmjena *odjeljenja* za menadžera ili *ID kase, početka i kraja smjene* za blagajnika.  

&nbsp;&nbsp;&nbsp;&nbsp;Primjenjuju se ista pravila i ograničenja kao kod unosa novog korisnika. Polja vrše automatsku provjeru unosa i onemogućavaju spremanje dok vrijednosti nisu ispravne. Tek nakon unosa validnih podataka izmjene se mogu potvrditi klikom na dugme ili pritiskom na *Enter*.

### Promjena tipa korisnika (Menadžer ↔ Blagajnik)

Menadžer može ***promijeniti ulogu*** odabranog korisnika klikom na odgovarajuću ikonicu. Prije izvršenja promjene, sistem traži potvrdu od menadžera. Ako se promjena potvrdi, uloga se ažurira, a sistem po potrebi popunjava ili resetuje polja specifična za novu ulogu.

&nbsp;&nbsp;&nbsp;&nbsp;Promjena vlastite uloge, kao ni uloge neaktivnog naloga, nije dozvoljena. U oba slučaja prikazuje se dijalog sa odgovarajućim upozorenjem.

### Aktiviranje / deaktiviranje korisnika
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer može privremeno ***onemogućiti*** ili ponovo ***omogućiti*** pristup korisnikovom nalogu bez brisanja podataka. Prije promjene statusa sistem traži potvrdu kako bi se spriječile neželjene greške.

&nbsp;&nbsp;&nbsp;&nbsp;Neaktivni korisnik više ne može pristupiti sistemu i prilikom pokušaja prijave dobija odgovarajuću poruku.

&nbsp;&nbsp;&nbsp;&nbsp;Nad nalogom deaktiviranog korisnika je takođe onemogućeno izvođenje određenih akcija od strane menadžera, poput resetovanja lozinke ili promjene uloge, sve dok nalog ne bude ponovo aktiviran.

&nbsp;&nbsp;&nbsp;&nbsp;Nije dozvoljeno deaktivirati vlastiti nalog. Takav pokušaj prikazuje dijalog sa jasno navedenim upozorenjem da je data operacija zabranjena.

### Resetovanje lozinke
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer može ***generisati privremenu lozinku*** za odabranog korisnika. Nakon uspješnog resetovanja, prikazuje se poruka sa privremenom lozinkom koju menadžer može proslijediti korisniku.

&nbsp;&nbsp;&nbsp;&nbsp;Prilikom sljedeće prijave, korisnik unosi privremenu lozinku, a zatim je obavezan da postavi novu, sopstvenu lozinku za buduće prijave.

&nbsp;&nbsp;&nbsp;&nbsp;Nije dozvoljeno resetovati vlastitu lozinku niti lozinku neaktivnog naloga.

## Upravljanje proizvodima

### Prikaz proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer ima mogućnost pregleda svih proizvoda koji čine asortiman poslovnog objekta, uz jasno organizovan prikaz za lakše upravljanje. U pregledu se prikazuju ***osnovne informacije*** o svakom artiklu: *ID, naziv, tip proizvoda* (*Hrana, Piće, Pribor*), *količina na stanju*, kao i dostupne akcije kroz ikonice za *detaljan pregled*, *uređivanje podataka* o odabranom proizvodu, *promjenu statusa* ili otvaranje prozora za *upravljanje zalihama*.

&nbsp;&nbsp;&nbsp;&nbsp;***Polje za pretragu*** omogućava brzo pronalaženje artikala (*Tab* ili *F3* prebacuju fokus na pretragu). Za ***filtriranje*** se koriste padajući meni za odabir ***po tipu proizvoda*** (*Ctrl + F*) i ***meni Atributi*** (*Ctrl + L*), sa opcijama poput *Sniženo*, *Istaknuto* i *Dostupno*.

### Dodavanje novih proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Dugme ***Dodaj*** u gornjem desnom uglu glavnog ekrana otvara prozor sa formularom za kreiranje novog proizvoda (*Ctrl + N*).  
Forma obuhvata osnovna polja koja se popunjavaju za sve proizvode, dok se dodatna, specifična polja prikazuju u zavisnosti od odabranog tipa (Hrana, Piće ili Pribor).

**Zajednička polja**

-   *Tip proizvoda* - padajući meni sa obaveznim izborom: *Hrana*, *Piće* ili *Pribor*,
-   *Naziv* - tekstualno polje za unos imena proizvoda uz provjeru minimalnog i maksimalnog broja karaktera,
-   *Opis* - prošireno tekstualno polje za detaljan opis,
-   *Cijena* - numeričko polje koje mora biti popunjeno i ne smije sadržati negativnu vrijednost,
-   *Slika proizvoda* - dugme za odabir fotografije; nakon izbora prikazuje se pregled slike. Podržani formati: *.jpg, .jpeg, .png*.

**Polja za tip Hrana**

&nbsp;&nbsp;&nbsp;&nbsp;Prikazuju se samo kada je odabran tip *Hrana*:

-   *Vrsta hrane* - padajući meni sa izborom kategorije (*torte, slatkiši, peciva, pekarski proizvodi*),
-   *Težina* - numeričko polje (izražava se u gramima),
-   *Kalorije* - numeričko polje koje očekuje cijeli broj,
-   *Lako kvarljivo* - izborno polje (checkbox) koje označava da proizvod zahtijeva prioritet u zalihama.

**Polja za tip Piće**

&nbsp;&nbsp;&nbsp;&nbsp;Prikazuju se samo kada je odabran tip *Piće*:

-   **Zapremina* -numeričko polje (u mililitrima), vrijednost ne može biti negativna,
-   **Alkoholno* - izborno polje (checkbox) koje označava da li proizvod sadrži alkohol.

**Polja za tip *Pribor***

&nbsp;&nbsp;&nbsp;&nbsp;Prikazuju se samo kada je odabran tip *Pribor*:

-   *Materijal* - obavezno tekstualno polje (npr. plastika, metal),
-   *Dimenzije* - tekstualno polje za unos u formatu "*broj*" ili "*broj × broj*", pri čemu brojevi mogu biti cijeli ili decimalni, a razmak je dozvoljen,
-   *Višekratno* - izborno polje (checkbox) koje označava mogućnost ponovne upotrebe.

&nbsp;&nbsp;&nbsp;&nbsp;Dugme ***Dodaj*** pokreće provjeru svih vidljivih polja. Ako su podaci ispravni, novi proizvod se sprema u bazu i prikazuje se informativna poruka o uspjehu. Ako postoje greške, korisnik dobija upozorenje, a neispravna polja bivaju jasno označena.  
&nbsp;&nbsp;&nbsp;&nbsp;Dugme ***Otkaži*** zatvara formu bez čuvanja promjena.

### Prikaz detalja proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer ima mogućnost detaljnog ***pregleda odabranog proizvoda***, sa svim relevantnim informacijama prikazanim na jednom mjestu. Pored osnovnih podataka (*naziv, tip, opis, cijena* i *popust*), prikazuju se i *datumi kreiranja* i *posljednje izmjene*, kao i vizuelni prikaz *slike proizvoda* radi lakše identifikacije.  

&nbsp;&nbsp;&nbsp;&nbsp;Prozor sadrži oznake (chipove) koje jasno ističu status proizvoda: *dostupan, nedostupan* ili *istaknut*.

&nbsp;&nbsp;&nbsp;&nbsp;Specifični atributi prikazuju se zavisno od tipa proizvoda: za hranu (*vrsta hrane, težina, kalorije, kvarljivost*), za piće (*zapremina, alkoholno/bezalkoholno*), a za pribor (*materijal, dimenzije, višekratna upotreba*).

### Uređivanje osnovnih podataka proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Klikom na odgovarajuću ikonicu za odabrani proizvod otvara se modalni prozor za ***izmjenu*** njegovih ***osnovnih podataka***. U ovom prozoru menadžer može mijenjati **cijenu proizvoda*, dodavati ili uklanjati *popust*, te označiti proizvod kao *istaknuti*. Polja prikazuju postojeće vrijednosti i omogućavaju direktnu izmjenu.

&nbsp;&nbsp;&nbsp;&nbsp;Primjenjuju se ista pravila i ograničenja kao kod dodavanja novog proizvoda. Polja vrše automatsku provjeru unosa i onemogućavaju
spremanje dok vrijednosti nisu ispravne (npr. cijena i popust moraju biti numerički podaci, ne smiju biti negativni). Tek nakon unosa validnih podataka izmjene se mogu potvrditi klikom na dugme ***Sačuvaj*** ili pritiskom na *Enter*, dok dugme ***Otkaži*** zatvara prozor bez promjena.

### Uređivanje detalja proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Dostupna je opcija za ***uređivanje profila proizvoda***. Potrebno je odabrati proizvod i kliknuti na odgovarajuću ikonicu.

&nbsp;&nbsp;&nbsp;&nbsp;Omogućeno je mijenjanje *naziva, opisa, slike*, kao i specifičnih atributa u zavisnosti od tipa proizvoda.

&nbsp;&nbsp;&nbsp;&nbsp;Sistem automatski provjerava tačnost unosa i ne dozvoljava čuvanje dok podaci nisu validni. Kada su sve vrijednosti ispravne, izmjene se mogu potvrditi klikom na dugme ***Sačuvaj*** ili pritiskom na *Enter*.

### Aktiviranje / deaktiviranje proizvoda
&nbsp;&nbsp;&nbsp;&nbsp;Menadžer može privremeno ***sakriti*** ili ponovo ***prikazati*** proizvod u aktivnoj listi bez brisanja podataka. Prije promjene statusa sistem traži potvrdu radi sprječavanja grešaka.

&nbsp;&nbsp;&nbsp;&nbsp;Neaktivni proizvod se ne prikazuje u standardnom pregledu i nije dostupan za dodavanje u prodajne transakcije dok se ponovo ne aktivira. Neke radnje, poput uređivanja cijene, popusta ili istaknutog statusa, takođe su onemogućene dok proizvod nije aktivan.

&nbsp;&nbsp;&nbsp;&nbsp;Deaktiviranje već arhiviranog proizvoda nije dozvoljeno; u tom slučaju pojavljuje se dijalog sa upozorenjem.

## Upravljanje zalihama

&nbsp;&nbsp;&nbsp;&nbsp;Klikom na odgovarajuću ikonicu pored željenog proizvoda otvara se prozor za ***upravljanje zalihama***, u kojem menadžer može detaljno pratiti i ažurirati stanje zaliha. Prikazuju se *količine, serije nabavki* i *rok trajanja* proizvoda, uz automatske indikatore koji jasno ističu kritične stavke.

&nbsp;&nbsp;&nbsp;&nbsp;Na ekranu je istaknuto *ime proizvoda* čije se zalihe uređuju, kao i *ukupna količina* trenutno dostupnih stavki.

&nbsp;&nbsp;&nbsp;&nbsp;Centralni dio ekrana prikazuje sve serije zaliha za izabrani proizvod, uključujući *ID serije, količinu, datum dodavanja* i *rok trajanja*.

&nbsp;&nbsp;&nbsp;&nbsp;Menadžer može potvrtiti izvršene promjene koristeći dugme ***Sačuvaj***, poništiti izmjene sa dugmetom ***Odbaci*** ili ukloniti selektovanu stavku klikom na dugme ***Obriši***.

&nbsp;&nbsp;&nbsp;&nbsp;Dostupan je i klizač koji omogućava ***filtriranje*** serija zaliha prema njihovom statusu (*aktivno / neaktivno*).

&nbsp;&nbsp;&nbsp;&nbsp;Desni panel prikazuje ***detalje*** izabrane stavke, uključujući *količinu, datum isteka, broj dana za upozorenje* i *datum dodavanja*. Polja imaju automatsku validaciju:

-   *Količina:* - obavezno polje, mora biti cio broj veći od nule,
-   *Datum isteka:* - opciono; ako je unet, mora biti najmanje 1 dan u budućnosti,
-   *Dani upozorenja:* - opciono; cio broj, ne može biti negativan,
-   *Datum dodavanja:* - prikazuje datum i vrijeme kada je stavka dodata,
-   *Aktivno:* - čekboks za aktivaciju ili deaktivaciju stavke.

&nbsp;&nbsp;&nbsp;&nbsp;Ispod panela nalazi se dugme ***Dodaj zalihe*** za kreiranje nove stavke, kao i dugme ***Zatvori*** koje zatvara ekran bez čuvanja promjena.

&nbsp;&nbsp;&nbsp;&nbsp;Ako datum isteka postoji i nalazimo se unutar broja dana za upozorenje, data stavka se automatski označava na poseban način (drugačija boja stavke) što omogućava menadžeru prepoznavanje kritičnih zaliha i pravovremenu reakciju.

&nbsp;&nbsp;&nbsp;&nbsp;Ovaj interfejs pruža sveobuhvatnu kontrolu nad zalihama proizvoda, omogućavajući precizno praćenje svake serije nabavke. Daje detaljan uvid u status svih stavki kako aktivnih tako i svih neaktivnih stavki bilo da im je promjenjen status, da je istekao rok ili da su jednostavno
potrošene. Na taj način menadzer ima mogućnost preciznog vođenje evidencije i analize o svim zalihama proizvoda.

## Statiskika i izvještaji

### Proizvodi

&nbsp;&nbsp;&nbsp;&nbsp;Ova sekcija menadžeru omogućava brz i pregledan uvid u ključne poslovne pokazatelje kroz intuitivne panele i grafičke prikaze.

&nbsp;&nbsp;&nbsp;&nbsp;Na ekranu se nalaze polja za odabir datuma ***„Od"*** i ***„Do"***, kao i dugme ***„Statistika"*** za generisanje podataka za željeni period. 

&nbsp;&nbsp;&nbsp;&nbsp;Polja za datume su opciona:

-   Prazno polje znači da nema vremenskog ograničenja za taj kraj perioda.
-   Ako je unesen samo datum „Od", prikazuju se podaci od tog datuma pa nadalje.
-   Ako je unesen samo datum „Do", prikazuju se podaci zaključno s tim datumom

&nbsp;&nbsp;&nbsp;&nbsp;Pritiskom na ***„Statistika"***, sistem prvo provjerava ispravnost unesenih datuma. Ako su validni, podaci se učitavaju i ažuriraju svi paneli, grafikoni i lista najprodavanijih proizvoda. U slučaju neispravnog unosa (npr. datum u budućnosti ili pogrešan interval), polja
se označavaju greškom i pojavljuje se dijalog upozorenja. Pretraga se neće izvršiti dok se greške ne isprave.

&nbsp;&nbsp;&nbsp;&nbsp;Ispod trake za filtriranje nalaze se informativni paneli sa pregledom najvažnijih metrika:

-   **Narudžbe:** prikaz ukupnog broja narudžbi, dodatno grupisan po statusima: *Završene, Otkazane* i *Na čekanju*.
-   **Artikli:** pregled ukupnog broja artikala, razvrstanih po kategorijama: *Hrana, Piće* i *Pribor*.
-   **Prihod:** ukupan prihod ostvaren u odabranom periodu.
-   
&nbsp;&nbsp;&nbsp;&nbsp;Dostupni su i grafikoni koji olakšavaju praćenje kretanja:

-   **Linijski grafikon:** prikazuje tendenciju prodaje artikala i broja narudžbi na nedeljnom nivou.
-   **Stubičasti grafikon:** prikazuje kretanje prihoda u istom periodu.

&nbsp;&nbsp;&nbsp;&nbsp;Na ekranu se takođe nalazi lista ***najprodavanijih proizvoda*** sa sličicama i nazivima, sortirana po broju prodatih artikala.

### Zalihe

&nbsp;&nbsp;&nbsp;&nbsp;Sekcija *Zalihe* omogućava menadžeru potpuni pregled stanja proizvoda u skladištu i svih promjena koje su izvršene.

&nbsp;&nbsp;&nbsp;&nbsp;Na početku korisnik: bira proizvod, definiše vremenski period putem polja „*Od*" i „*Do*", pokreće generisanje izvještaja klikom na dugme ***Izvještaj***.

&nbsp;&nbsp;&nbsp;&nbsp;Sistem automatski provjerava validnost unesenih podataka:
-   datum ne smije biti u budućnosti,
-   krajnji datum mora biti nakon početnog,
-   izbor proizvoda je obavezan za detaljne izvještaje.

&nbsp;&nbsp;&nbsp;&nbsp;Nepravilno uneseni datumi ili neizabrani proizvod onemogućavaju generisanje izvještaja i prikazuju vizuelni indikator greške pored
polja.

**Tabovi:**

-   ***Zalihe*** - prikazuje stavke kojima ističe rok trajanja, nezavisno od izbora proizvoda i perioda. Tabela sadrži *ID zaliha, ID proizvoda, naziv proizvoda, količinu, datum isteka* i *broj dana do upozorenja*,
-   ***Transakcije*** - prikazuje sve promjene stanja zaliha za odabrani proizvod i period. Evidentiraju se *ID transakcije, datum i vrijeme, tip transakcije* (*prijem, prodaja, korekcija*), *promjena količine, broj narudžbine* i *ID korisnika*,
-   ***Izmjene*** - detaljan pregled svih promjena u zalihama, uključujući *datum, tip izmjene, prethodnu i novu vrijednost*, te *ID korisnika*,
-   ***Sažetak*** - ukupni pregled za odabrani proizvod i period: *količina na skladištu, dodate i prodate količine*, te *ukupni broj izmjena.

&nbsp;&nbsp;&nbsp;&nbsp;Tabele podržavaju sortiranje, a prazni prikazi uvijek obavještavaju korisnika da nema podataka za trenutne kriterijume.

&nbsp;&nbsp;&nbsp;&nbsp;Ovakav tok, od izbora proizvoda i datuma, preko generisanja izvještaja, do preglednog prikaza zaliha, transakcija, izmjena i sažetka, omogućava menadžeru efikasno praćenje stanja skladišta i donošenje informisanih odluka o poslovanju objekta koji koristi PastryShop aplikaciju.

# Blagajnik

&nbsp;&nbsp;&nbsp;&nbsp;Uloga blagajnika je da osigura brzo i precizno upravljanje prodajom i naplatom u pekarskom ili slastičarskom objektu.  

&nbsp;&nbsp;&nbsp;&nbsp;Glavne odgovornosti blagajnika su:

- kreiranje i naplata porudžbina,
- praćenje i pregled porudžbina,
- pregled finansijskih i operativnih izvještaja,
- upravljanje ličnim postavkama i profilom.

### Prijava i pristup

Po uspješnoj prijavi na sistem prikazuje se ***početni ekran za blagajnika (POS terminal)***.

&nbsp;&nbsp;&nbsp;&nbsp;U gornjem dijelu prikazana je *profilna slika* i *korisničko ime* blagajnika, što jasno pokazuje koji je korisnički nalog trenutno prijavljen.

&nbsp;&nbsp;&nbsp;&nbsp;Na lijevoj strani ekrana nalazi se vertikalni navigacioni meni, koji omogućava brz pristup ključnim modulima sistema. Svaka opcija može se brzo otvoriti pomoću tastaturnih prečica.

&nbsp;&nbsp;&nbsp;&nbsp;Dostupne su sljedeće funkcionalnosti:

-   ***POS (Blagajna):*** - osnovni ekran za kreiranje i naplatu porudžbina (*Ctrl + P*),
-   ***Porudžbine:*** - pregled svih transakcija i detalja o njima (*Ctrl + O*),
-   ***Izveštaji:*** - finansijski i operativni podaci vezani za rad blagajnika (*Ctrl + R*),
-   ***Podešavanja:*** - lične postavke naloga, uključujući jezik i temu (*Ctrl + S*),
-   ***Profil:*** - uređivanje ličnih podataka korisnika poput imena i lozinke (*Ctrl + M*),
-   ***Odjava:*** - izlazak iz aplikacije i povratak na login ekran (*Escape*).

&nbsp;&nbsp;&nbsp;&nbsp;Glavni dio ekrana prikazuje sadržaj izabranog modula. Sve funkcionalnosti odmah su dostupne, bez potrebe za dodatnom navigacijom.

## POS ekran

### Pretraga

&nbsp;&nbsp;&nbsp;&nbsp;U gornjem dijelu ekrana dostupni su alati za filtriranje i brzu pretragu proizvoda: padajući meni za izbor tipa proizvoda: hrana, piće, pribor (prečica je *Ctrl + T*), tekstualno polje za pretragu po nazivu ili tipu proizvoda (fokus na dato polje se aktivira pritiskom na *F3*), te tri prekidača za filtriranje: Dostupno (*Ctrl + A*), Istaknuto (*Ctrl + F*) i Sniženo (*Ctrl + D*). Kombinacijom navedenih opcija korisnik može
jednostavno fokusirati prikaz na željene artikle.

### Lista proizvoda

&nbsp;&nbsp;&nbsp;&nbsp;U centralnom dijelu ekrana prikazani su proizvodi kao kartice koje se mogu slobodno skrolovati.

&nbsp;&nbsp;&nbsp;&nbsp;Svaka kartica sadrži: *naziv proizvoda, opis* (kao tooltip), *cijenu*, kao i dugme za dodavanje proizvoda u korpu ("+").  
Pritiskom na „***+***" sistem provjerava da li je proizvod dostupan, da li ga ima na stanju, te sprečava dodavanje istog proizvoda više puta.  
U slučaju da proizvod nije dostupan za prodaju, da nema dovoljno zaliha ili je već u korpi, prikazuje se kratko obavještenje o tome.

&nbsp;&nbsp;&nbsp;&nbsp;Ukoliko je dodavanje uspješno, korpa se odmah osvježava i prikazuje dodati proizvod.

### Korpa

&nbsp;&nbsp;&nbsp;&nbsp;U korpi se prikazuju svi odabrani proizvodi sa *slikom*, *nazivom* i *cijenom*. *Količina* se može mijenjati pomoću *plus/minus* dugmadi, a postoji i dugme za uklanjanje proizvoda iz korpe.

&nbsp;&nbsp;&nbsp;&nbsp;Promjene količine se odmah odražavaju na cijenu proizvoda i ukupan iznos korpe.

&nbsp;&nbsp;&nbsp;&nbsp;Pokušaj povećanja količine iznad raspoloživog stanja prikazuje kratkotrajni vizualni indikator (popup) koji signalizira da je dostignut limit zaliha. Uklanjanjem proizvoda iz korpe ukupan iznos se automatski osvježava.

&nbsp;&nbsp;&nbsp;&nbsp;Za svaku od opcija plaćanja traži se potvrda kroz dijalog.

&nbsp;&nbsp;&nbsp;&nbsp;Ako je operacija uspješna, korpa se prazni i prikazuje se odgovarajuća informativna poruka o uspjehu.

&nbsp;&nbsp;&nbsp;&nbsp;Ako je korpa prazna pri pokušaju plaćanja, korisnik dobija odgovarajuće obavještenje.

### Ukupan iznos i opcije plaćanja

&nbsp;&nbsp;&nbsp;&nbsp;Ispod liste dodatih proizvoda prikazuje se ukupni iznos korpe uz oznaku valute.

&nbsp;&nbsp;&nbsp;&nbsp;Blagajniku su dostupne tri opcije plaćanja:

- ***Kasnije*** (*Pay Later*) - plaćanje se odlaže (*Ctrl + L*),
- ***Keš*** (*Pay Cash*) - plaćanje se vrši gotovinom (*Ctrl + G*),
- ***Kartica*** (*Pay Card*) - otvara se dijalog za unos broja kartice; nakon potvrde evidentira se plaćanje karticom (*Ctrl + N*).

&nbsp;&nbsp;&nbsp;&nbsp;Bez obzira na odabranu opciju, svaka transakcija kreira porudžbinu, evidentira plaćanje i smanjuje količine proizvoda u zalihama.

&nbsp;&nbsp;&nbsp;&nbsp;Radi dodatne sigurnosti, potrebno je potvrditi operaciju kroz dijalog.

&nbsp;&nbsp;&nbsp;&nbsp;Ako je operacija bila uspješna, korpa se prazni i prikazuje se adekvatna poruka. Ukoliko je korpa bila prazna prilikom pokušaja plaćanja, korisnik dobija odgovarajuće obavještenje.

## Porudžbine.

&nbsp;&nbsp;&nbsp;&nbsp;U centralnom dijelu ekrana nalazi se padajući meni za filtriranje porudžbina na osnovu tipa (Sve, Kompletirane, Otkazane, Na čekanju). Za pristup meniju moguće je koristiti prečicu *Ctrl + T*.

&nbsp;&nbsp;&nbsp;&nbsp;Sve porudžbine koje je blagajnik realizovao u posljednja 24 časa su prikazane u tabeli sa kolonama: *ID, Datum, Ukupna cijena* i *Status*. Tabela podržava sortiranje kolona. U slučaju da nema porudžbina umjesto tabele se prikazuje poruka: "Nema podataka".

&nbsp;&nbsp;&nbsp;&nbsp;Na desnoj strani ekrana se nalazi bočni panel za prikaz detalja odabrane porudžbine. Odabirom reda u tabeli desni panel učitava i prikazuje stavke te porudžbine što uključuje: *naziv proizvoda, količinu, cijenu po jedinici* i *ukupnu cijenu stavke*.

&nbsp;&nbsp;&nbsp;&nbsp;Ispod popisa stavki prikazan je ukupni iznos izabrane porudžbine.

&nbsp;&nbsp;&nbsp;&nbsp;Za odabranu porudžbinu su omogućene sledeće funkcionalnosti:

- ***Otkaži*** (*Cancel*) - *Ctrl + L*
  
&nbsp;&nbsp;&nbsp;&nbsp;Dozvoljeno je da se otkažu već kreiran porudžbine. Prikazuje se dijalog za potvrdu poslije čega se vraćaju zalihe artikala za sve stavke porudžbine.
U slučaju da je porudžbina bila na čekanju, njen status postaje "*Otkazana*", a u slučaju da je već bila kompletirana status postaje "*Refundirana*" i izvršava se povrat sredstava (stanje novca se umanjuje).
Nije dozvoljeno ponovo otkazati već otkazanu porudžbinu.

- ***Gotovina*** (*Pay Cash*) - *Ctrl + G*

&nbsp;&nbsp;&nbsp;&nbsp;Plaćanje gotovinom primjenjuje se na porudžbine koje su u statusu "*Na čekanju*".
Ne može se platiti otkazana ili već plaćena porudžbina. Aplikacija sprječava takav pokušaj i prikazuje odgovarajući dijalog.

- ***Kartica*** (*Pay Card*) - *Ctrl + N*

&nbsp;&nbsp;&nbsp;&nbsp;Plaćanje karticom takođe se primjenjuje na porudžbine koje su u statusu "*Na čekanju*".

Otvara se dijalog za unos broja kartice. Unos je obavezan i podliježe validaciji. Nakon potvrde evidentira se plaćanje i porudžbina se ažurira. Isto kao kod gotovine, ne može se platiti otkazana ili već plaćena porudžbina. U slučaju da se pokuša izvršiti otkazivanje ili plaćanje bez odabrane porudžbine, pojavljuje se dijalog o nedozvoljenoj operaciji. Za svaku uspješno izvršenu operaciju prikazuju se adekvatne informativne poruke.

## Statistika i izvještaji

&nbsp;&nbsp;&nbsp;&nbsp;Ekran ***Izvještaji*** omogućava blagajniku brz pregled ključnih metrika kroz intuitivne panele i grafičke prikaze.
Svi prikazi odnose se na konkretnog blagajnika (prema korisničkom ID-u) za tekući period.

&nbsp;&nbsp;&nbsp;&nbsp;U gornjem dijelu ekrana prikazani su podaci o ukupnom broju porudžbina, ukupno ostvarenim stavkama i ukupnom prihodu generisanom tokom rada prijavljenog korisnika.

&nbsp;&nbsp;&nbsp;&nbsp;Donji lijevi dio sadrži dnevne statistike: broj današnjih porudžbina grupisan po statusima (Kompletirane, Otkazane, Na čekanju), ostvareni dnevni prihod i broj prodatih artikala raspoređenih po kategorijama (hrana, piće, pribor) prikazanih. U slučaju nedostatka podataka, umjesto grafikona prikazuje se poruka *nema porudžbina*.

&nbsp;&nbsp;&nbsp;&nbsp;Linijski grafikon prikazuje sedmični broj stavki i porudžbina vezanih za rad blagajnika, dok stubičasti grafikon prikazuje kretanje prihoda u istom periodu.

&nbsp;&nbsp;&nbsp;&nbsp;Na ekranu se nalazi i lista najprodavanijih proizvoda.

# Zajedničke funkcionalnosti

&nbsp;&nbsp;&nbsp;&nbsp;Ovo poglavlje opisuje funkcije dostupne i menadžeru i blagajniku:
postavke jezika i teme, te uređivanje korisničkog profila.

## Postavke (Jezik i Tema)

&nbsp;&nbsp;&nbsp;&nbsp;Aplikacija PastryShop omogućava korisnicima potpunu personalizaciju izgleda i jezika korisničkog interfejsa. Na taj način se osigurava ugodno i intuitivno iskustvo tokom rada.

Dostupne opcije:

&nbsp;&nbsp;&nbsp;&nbsp;Teme interfejsa: korisnik može birati između ***svijetle***, ***tamne*** i ***plave*** teme.

&nbsp;&nbsp;&nbsp;&nbsp;Jezici: trenutno su podržani ***srpski*** i ***engleski*** jezik.

&nbsp;&nbsp;&nbsp;&nbsp;Padajući meniji za izbor jezika i teme sadrže odgovarajuće ikonice uz svaku opciju, dok su nazivi prilagođeni trenutno aktivnom jeziku aplikacije, čime se postiže dodatna jasnoća.

&nbsp;&nbsp;&nbsp;&nbsp;Klikom na dugme ***Sačuvaj*** aplikacija upoređuje odabrane postavke jezika i teme sa prethodnim vrijednostima i odmah primjenjuje eventualne
promjene. Omogućeno je promijeniti samo temu, samo jezik, ili oboje istovremeno.

&nbsp;&nbsp;&nbsp;&nbsp;Korisnik dobija informativnu poruku o rezultatu („Tema i jezik promijenjeni", „Tema promijenjena", „Jezik promijenjen" ili „Nema promjena").

&nbsp;&nbsp;&nbsp;&nbsp;Odabrane postavke (tema i jezik) se automatski čuvavaju za datog korisnika pa su pri svakoj njegovoj narednoj prijavi dostupne prethodno odabrane opcije.

##  Profil korisnika

&nbsp;&nbsp;&nbsp;&nbsp;Prilikom otvaranja stranice Profil korisnika, trenutni podaci su već učitani, što omogućava pregled i eventualnu izmjenu postojećih informacija.

&nbsp;&nbsp;&nbsp;&nbsp;Korisnik može promijeniti sljedeće podatke:

Korisničko ime: obavezno polje; dozvoljeni znakovi su slova i cifre,
postoji ograničenje dužine, a ime mora biti jedinstveno.

Lozinka: opciono; ako se unese, primjenjuju se ista pravila kao za
korisničko ime. Lozinka je maskirana radi sigurnosti.

Broj telefona: obavezno polje; uneseni broj mora odgovarati formatu
telefonskog broja.

Adresa: polje bez dodatnih ograničenja.

Izaberite sliku: otvara dijalog za odabir slike; podržani formati su
\`jpg\`, \`jpeg\` i \`png\`.

Dugmad:

Sačuvaj (Ctrl + S) -- vrši validaciju unesenih vrijednosti i u slučaju
da su podaci ispravni, čuva izmjene u profilu korisnika.

Poništi (Ctrl + R) -- vraća prethodno aktivne vrijednosti, poništavajući
sve nedavne izmjene.

## Responzivni dizajn

&nbsp;&nbsp;&nbsp;&nbsp;Pastry Shop aplikacija je dizajnirana sa fokusom na responzivni dizajn, osiguravajući ugodno korisničko iskustvo na desktop i laptop uređajima. Interfejs se automatski prilagođava veličini i rezoluciji ekrana, tako da sadržaj uvek izgleda i funkcioniše besprekorno.

Na većim ekranima sadržaj koji uključuje liste proizvoda ili navigacione ikonice organizovan je u više kolona za maksimalnu iskorišćenost prostora, dok se na manjim ekranima automatski prebacuje na manji broj kolona. Po potrebi se pojedine slike mogu sakriti na manjim širinama ekrana kako bi se uštedio prostor i sačuvao pregledan raspored.
