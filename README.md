# Metrobüsler için daha etkili bir çalışma önerisi
Bu proje metrobüsler için daha etkin bir çalışma önerisi ve bu önerinin faydasını analiz etmek için hazırlanmış simulasyon programını içermektedir.

PROBLEM: Metrobüs istanbul ulaşımının omurgası haline geldi. Özellikle mesai saatlerinde neredeyse kesintisiz peşpeşe otobüsler hareket ediyor. 
Buna rağmen oluşan talep zor karşılanıyor. Metrobüs hattı daha fazla otobüs kaldırmadığı için talebi karşılamak için daha fazla otobüs eklenemiyor. 
Bunca peşpeşe çalışan metrobüse rağmen yatay doğrultuda genişlemiş olan istanbulda avcılardan zincirli kuyuya gitmek 1 Saati buluyor. 

ÖNERİ: 
Metrobüs durakları mavi/kırmızı duraklar olarak sırayla işaretlenir. Bir uçtan bir uca tüm durakların mavi-kırmızı-mavi-kırmızı.. olarak sırayla renklendirilmesi sağlanır. 
Zincirlikuyu, kadıköy gibi yoğun duraklar ise yeşil renk ile renklendirilir. 
Metrobüsler de duraklar gibi mavi veya kırmızı renk ile işaretlenir.  
Her metrobüs kendi rengindeki duraklarda ve yeşil renge sahip duraklarda durur.  

Örnek: Hattın birinci durağı kırmızı, ikinci durağı mavi olsun. Bu şekilde 3. kırmızı, 4. mavi şeklinde devam etsin.  
Birinci duraktan ve ikinci duraktan mavi ve kırmızı renkte iki otobüs hareket etmiş olsun. 
Otobüsler peş peşe hareket edecek, birinci duraktan hareket eden otobüs 3. durakta(kırmızı); ikinci duraktan hareket eden otobüs 4. durakta duracaktır.  
Kırmızı otobüs mavi durakta durmayacak, mavi otobüs kırmızı durakta durmayacaktır. Zincirlikuyu durağı yeşil renk(yoğun durak) olduğu için her iki otobüs de bu durakta duracaktır. Otobüsler peşpeşe birbirlerini geçmeden birer durak atlayarak yol alacaklar. 

Böylece beylikdüzünden hareket eden bir otobüs daha az durakta durarak daha kısa zamanda yolculuğunu tamamlayacaktır.  

Bu fikrin açık bir dezavantajı var: Örneğin kırmızı renk olan birinci duraktan binen bir yolcu mavi renk olan 10. durakta inemeyecek.  
Bu kısıtın olumsuz etkisi aşağıdaki önlemler ile minimize edilebilebilir: 
Metrobüs kullanan yolcular metrobüs hattına aşağıdaki şekillerde ulaşıp yolculuklarına metrobüs ile devam ediyorlar. 
* dolmuş/otobüs 
* kendi aracı 
* yürüyerek 

1. metrobüse erişim için kullanılan dolmuş/otobüs hatlarının hem mavi hem kırmızı duraklara uğraması sağlanır. Böylece dolmuş/otobüs yolcuları seyehatlarına uygun renkteki duraktan metrobüse binebilirler. 
2. Metrobüse kadar kendi aracı ile gidip yolculuğuna metrobüs ile devam edenler araçları ile bir önceki/bir sonraki durağa gidebilirler. Bunun için park imkanları iyileştirilmelidir. 
3. ek olarak yoğun kullanılan durakların yeşil (ortak) seçilmesi bu dezavantajın etkisini azaltacaktır. 

Ayrıca yoğun duraklarda diğer renge aktarma yaparak inilmek istenilen durağa uygun renkteki otobüse geçilebilir.

Bu önlemlere rağmen metrobüs kullananların ortalama yürüme süreleri artacaktır. Ancak metrobüste geçen toplam süre halkın büyük çoğunluğu için azalacağından önemli oranda fayda elde edilecektir.
Ayrıca elde edilecek sonuca göre bu önerinin yalnızca sabah ve akşam metrobüsün yoğun olduğu zamanlarda uygulanması, diğer zamanlarda mevcut şekilde devam etmesi düşünülebilir.


