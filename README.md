# Diacritics

### What it is
Diacritics reconstruction (restoration) for Slovak text based on finding best match in n-grams (n-gram = group of n words usually occurring together in language). 
This program was created for Bachelor's thesis at [Faculty of Management Science and Informatics](https://www.fri.uniza.sk/en/), [University of Žilina](https://www.uniza.sk/index.php/en/). 

### How it works
The program uses data from [Slovak National Corpus](https://korpus.sk/index_en.html) from Ľ. Štúr Institute of Linguistics, Slovak Academy of Sciences. We used data set/language corpus [prim-8.0-public-all](https://korpus.sk/prim(2d)8(2e)0_en.html) made out of 1.5 billion of tokens (namely subcorpuses of 4-grams, 3-grams, 2-grams and words). Yout can find them all [here](https://korpus.sk/files/prim-8.0/).
Algorithm reconstructs every single word separately. It uses data structure [trie](https://en.wikipedia.org/wiki/Trie) for fastest access to the list of appropriate n-grams for each non-diacritics word. List of appropriate n-grams for non-diacritics word consists only of n-grams containing that word. In addition, the list is grouped by n (from 4-grams to 1-gram) and sorted by absolute occurrence in language. Then all n-grams are compared with the word and it's surrouding words one by one until there is match. After then the word is being replaced with found diacritics form.

### More information
- Bachelor's thesis in Slovak language: [Automatická rekonštrukcia diakritiky pre slovenčinu](https://opac.crzp.sk/?fn=detailBiblioForm&sid=BCA102CB6C4CA54D4AE7A475C35B&seo=CRZP-detail-kniha)
- Conference Paper in English: [Automatic restoration of diacritics based on word n-grams for Slovak texts](https://www.researchgate.net/publication/342255712_Automatic_restoration_of_diacritics_based_on_word_n-grams_for_Slovak_texts) or on [IEEE Xplore](https://ieeexplore.ieee.org/document/9119328)
- Article in English: [Diacritics restoration based on word n-grams for Slovak texts](https://www.researchgate.net/publication/349351309_Diacritics_restoration_based_on_word_n-grams_for_Slovak_texts) or on [De Gruyter](https://www.degruyter.com/document/doi/10.1515/comp-2020-0143/html)

### Used technologies
- C#
- ASP.NET Core
- [PBCD.DataStructures.Trie](https://www.nuget.org/packages/PBCD.DataStructures.Trie/)

### Final software
There are two final versions of the program: 
The first - faster one (0.4ms per word), using RAM only, with the success rate **98.07%**.
The second - slower one (4ms per word), using hard disk, with success rate **98.17%**.
Here you will find:
- DLL ready to use
- Simple web-site for easy, user-friendly interacting with the program

### Try it here
[diakritika.fri.uniza.sk](http://diakritika.fri.uniza.sk/)

#### To run it you need to download these files:
https://www.dropbox.com/s/7uraxif4ocfay8k/diacritics-reconstructor-necessary-files.zip?dl=0
