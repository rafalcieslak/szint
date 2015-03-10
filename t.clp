; Rafał Cieślak
; Zadanie z Regułowych Systemów Produkcji
; realizowane w ramach przedmiotu Sztuczna Inteligencja
; w ramach studiów na kierunku Informatyka
; na Uniwersytecie Wrocławskim
; semestr letni 2014/2015
;
; ============================================
; Program rozpoznający położenie geograficzne na podstawie prostych obserwacji
; otoczenia.
; ============================================
;
; Program zadaje użytkownikowi serię pytań dotczących obserwowanego otoczenia.
; Pytania symbolizują proste spostrzeżenia, większość z nich możnaby łatwo
; wykonać automatycznie za pomocą sprzętu pomiarowego.
; Na podstawie odpowiedzi program próbuje budować uproszczony model środowiska
; na który składają się różne fakty dotyczące otoczenia. Sporo z obserwacji
; można wywnioskować na podstawie wcześniejszych odpowiedzi - w miarę możliwości
; program stara się zadać jak najmniej pytań (by możliwie uprościć interakcję,
; lub by uniknąć dużej liczby pomiarów), i wnioskować z nich możliwie wiele
; faktów. Program podaje potencjalną lokalizację obserwatora (czasami jest
; to stwierdzenie ogólne np. "ocean", czasami dokładnie strefa klimatyczna,
; a czasami wręcz precyzyjne położenie geograficzne np. "pustynia Atakama") gdy
; tylko wprowadzone obserwacje umożliwiają na jej określenie.
; Skuteczność programu w większości zależy od stopnia rozbudowania wewnętrznej
; bazy możliwych położeń i określenia cech charakterystycznych dla różnych
; miejsc. W tej wersji program mniej więcej poprawnie rozpoznaje około 20
; różnych rejonów geograficznych. W przypadku wprowadzenia obserwacji
; pochodzących z innego rejonu, program może albo podać nieprecyzyjną odpowiedź,
; albo wskazać, że nie zna żadnego miejsca pasujęcego do wprowadzonego opisu.

(deffunction yesno (?question)
	(printout t ?question " [yes/no] ")
	(bind ?ans (read))
	(while (and (<> (str-compare ?ans "yes") 0) (<> (str-compare ?ans "no") 0))
		(printout t " Please answer yes or no." crlf)
		(bind ?ans (read))
	)
	?ans
)

(deffunction yesno2 ()
	(printout t " [yes/no] ")
	(bind ?ans (read))
	(while (and (<> (str-compare ?ans "yes") 0) (<> (str-compare ?ans "no") 0))
		(printout t " Please answer yes or no." crlf)
		(bind ?ans (read))
	)
	?ans
)

(deffunction ask ($?options)
	(bind ?ans (read))
	(while TRUE
		(progn$ (?pattern ?options)
			(if (= (str-compare ?pattern ?ans) 0) then
				(break)
			)
		)
		(printout t " Possible answers: " ?options crlf)
		(bind ?ans (read))
	)
	?ans
)

; ===== QUESTIONS ======

(defrule civ (declare (salience -10))
	(not (civilisation ?))
	=>
  (assert (civilisation(yesno "Can you spot any tracks of civilisation?")))
)

(defrule vegetation (declare (salience -10))
	(not (vegetation ?))
	=>
  (assert (vegetation(yesno "Is there any vegetation around you?")))
)

(defrule buildings (declare (salience -10))
	(not (buildings ?))
	=>
	(assert (buildings(yesno "Are there any buildings in your sight?")))
)
(defrule temph (declare (salience -10))
	(not (hot ?))
	=>
	(assert (hot(yesno "Is it particularly hot?")))
)
(defrule templ (declare (salience -10))
	(not (cold ?))
	=>
	(assert (cold(yesno "Is it particularly cold?")))
)
(defrule high-humid (declare (salience -10))
	(temperature high)
	(not (humidity-high ?))
	=>
	(assert (humidity-high(yesno "Is the humidity very high?")))
)
(defrule breathing (declare (salience -10))
	(not (breathing-hard ?))
	=>
	(assert (breathing-hard(yesno "Do you find it difficult to breathe?")))
)
(defrule manybuildings (declare (salience -10))
	(buildings yes)
	(not (manybuildings ?))
	=>
	(assert (manybuildings(yesno "Are the buildings you see plentiful?")))
)
(defrule skycrappers (declare (salience -10))
	(manybuildings yes)
	=>
	(assert (skycrappers(yesno "Are there skycrappers?")))
)
(defrule seefar (declare (salience -10))
	(not (seefar ?))
	=>
	(assert (seefar(yesno "Is your view range large (~ above 2km)?")))
)
(defrule lowlat (declare (salience -10))
	(not (lowlat ?))
	=>
	(assert (lowlat(yesno "Is the sun shining directly above you?")))
)
(defrule highlat (declare (salience -10))
	(not (highlat ?))
	=>
	(assert (highlat(yesno "Is the sun always very close to the horizon?")))
)
(defrule sand (declare (salience -10))
	(vegetation no)
	(not (sandeverywhere ?))
	=>
	(assert (sandeverywhere(yesno "Is all you see sand and gravel?")))
)
(defrule flat (declare (salience -10))
	(not (flat ?))
	=>
	(assert (flat(yesno "Is the landscape generally flat?")))
)
(defrule superflat (declare (salience -10))
	(flat yes)
	(not (superflat ?))
	=>
	(assert (superflat(yesno "Is the landscape completely flat?")))
)
(defrule iceeverywhere (declare (salience -10))
	(not (iceeverywhere ?))
	=>
	(assert (iceeverywhere(yesno "Is there ice everywhere you see?")))
)
(defrule snoweverywhere (declare (salience -10))
	(not (snoweverywhere ?))
	=>
	(assert (snoweverywhere(yesno "Is there snow everywhere you see?")))
)
(defrule trees (declare (salience -10))
	(vegetation yes)
	(not (trees ?))
	=>
	(assert (trees(yesno "Are there trees around you?")))
)
(defrule grass (declare (salience -10))
	(not (grass ?))
	=>
	(assert (grass(yesno "Can you see a lot of grass?")))
)
(defrule conifers (declare (salience -10))
	(trees yes)
	(not (conifers ?))
	=>
	(assert (conifers(yesno "Are all of the trees you see conifers?")))
)
(defrule underwater (declare (salience -10))
	(not (underwater ?))
	=>
	(assert (underwater(yesno "Are you submerged in water?")))
)
(defrule light (declare (salience -10))
	(not (light ?))
	=>
	(assert (light(yesno "Can you see any light (during daytime)?")))
)
(defrule canopy (declare (salience -10))
	(trees yes)
	(not (treeshade ?))
	=>
	(assert (treeshade(yesno "Are the trees dense enough to provide you with shade?")))
)
; ========= REASONING ========
(defrule build-civ (buildings yes) => (assert(civilisation yes)))
(defrule nociv-nobuild (civilisation no) => (assert(buildings no)))
(defrule hot
	(hot yes) =>
	(assert (temperature high) (cold no))
)
(defrule cold
	(cold yes) =>
	(assert (temperature low) (hot no))
)
(defrule not-hot-not-cold
	(hot no) (cold no) =>
	(assert (temperature neutral))
)
(defrule air-press-low
	(breathing-hard yes)
	(humidity-high no)
	=>
	(assert (air-pressure low))
)
(defrule low-humid-if-cold
	(temperature low) => (assert (humidity-high no))
)
(defrule alt2
	(see-far yes)
	(flat no)
	=>
	(assert (altitude high))
)
(defrule icecold
	(iceeverywhere yes)
	=>
	(assert (cold yes) (hot no) (temperature low))
)
(defrule lat1 (highlat yes) => (assert (lowlat no) (latitude high)))
(defrule lat2 (lowlat yes) => (assert (highlat no) (latitude low)))
(defrule lat3 (lowlat no) (highlat no) => (assert (highlat yes) (latitude medium)))
(defrule guesstemp
	(humidity-high yes)
	(breathing-hard yes)
	=>
	(assert (temperature high) (hot yes) (cold no))
)
(defrule air-high-alt
	(air-pressure low)
	=>
	(assert (altitude high))
)
(defrule highsee
	(altitude high)
	(flat no)
	=>
	(assert (seefar yes))
)
(defrule plants-are-not-sand
	(vegetation yes) => (assert (sandeverywhere no))
)
(defrule sand-is-not-a-plant
	(sandeverywhere yes) => (assert (vegetation no))
)
(defrule ice-is-not-plant
	(iceeverywhere yes) => (assert (vegetation yes))
)
(defrule ice-is-cold
	(or (iceeverywhere yes) (snoweverywhere yes))
	=>
	(assert (cold yes))
)
(defrule no-light-no-sun
	(light no) => (assert (lowlat no))
)
(defrule no-sun-at-poles
	(underwater no)
	(light no)
	=>
	(assert (highlat yes))
)
(defrule seefar-from-heights
	(flat no)
	(seefar yes)
	=>
	(assert (altitude high))
)
(defrule vegetation-trees-etc
	(or (grass yes) (trees yes))
	=>
	(assert (vegetation yes))
)
(defrule no-vegetation
	(vegetation no)
	=>
	(assert (grass no) (trees no))
)
(defrule water-properties
	(underwater yes)
	=>
	(assert (hot no) (seefar no) (altitude low) (high-humid yes) (breathing-hard yes) (trees no) (grass no) (snoweverywhere no))
)
(defrule no-snow-in-hot
	(temperature high)
	=>
	(assert (snoweverywhere no) (iceeverywhere no))
)
(defrule temp-synonym
	(temperature low) => (assert (temperature cold))
)
(defrule high-lat-cold
	(highlat yes) => (assert (cold yes))
)
(defrule low-lat-hot
	(lowlat yes)
	(altitude low)
  =>
	(assert (hot yes))
)

; ======== LOCATIONS =========

(deffunction location (?loc)
	(assert (found-location ?loc))
	(printout t "POSSIBLE LOCATION: " ?loc crlf)
	(printout t "Use (run) to search further, or (reset) (run) to retry." crlf)
	(halt)
)

(defrule final
	(declare (salience -50))
	(not(found-location ?))
	=>
	(printout t "No clues about your location are available.")
)

(defrule metropoly (declare (salience 10))
	(skycrappers yes)
	=>
	(location "Metropoly")
)
(defrule city (declare (salience 10))
	(skycrappers no)
	(manybuildings yes)
	=>
	(location "City")
)
(defrule dubai (declare (salience 10))
	(skycrappers yes)
	(sandeverywhere yes)
	=>
	(location "Dubai")
)

(defrule sand-desert (declare (salience 10))
	(flat yes)
	(superflat no)
	(lowlat yes)
	(sandeverywhere yes)
	(temperature high)
	(civilisation no)
	=>
	(location "Arabian or Sahara desert")
)

(defrule gobi-desert (declare (salience 10))
	(superflat yes)
	(temperature high)
	(civilisation no)
	(sandeverywhere yes)
	=>
	(location "Gobi desert")
)

(defrule antarctic-desert (declare (salience 10))
	(highlat yes)
	(superflat yes)
	(iceeverywhere yes)
	(temperature low)
	(civilisation no)
	=>
	(location "Antarctic desert")
)
(defrule arctic-desert (declare (salience 10))
	(highlat yes)
	(temperature low)
	(civilisation no)
	(vegetation yes)
	(trees no)
	=>
	(location "Arctic desert")
)
(defrule atacama-desert (declare (salience 10))
	(temperature low)
	(civilisation no)
	(altitude high)
	=>
	(location "Atacama desert")
)
(defrule taiga (declare (salience 10))
	(temperature low)
	(snoweverywhere yes)
	(conifers yes)
	=>
	(location "Taiga")
)
(defrule tropical-forest (declare (salience 10))
	(humidity-high yes)
	(trees yes)
	(temperature high)
	(treeshade yes)
	=>
	(location "Tropical forest")
)
(defrule atlantis (declare (salience 10))
	(civilisation yes)
	(underwater yes)
	=>
	(location "Atlantis")
)
(defrule deepocean (declare (salience 10))
	(underwater yes)
	(light no)
	=>
	(location "Deep ocean")
)
(defrule shallowwater (declare (salience 10))
	(underwater yes)
	(light yes)
	=>
	(location "Shallow water")
)
(defrule high-mountains (declare (salience 10))
	(altitude high)
	(snoweverywhere yes)
	=>
	(location "High mountains (4000+ m)")
)
(defrule savannah (declare (salience 10))
	(temperature high)
	(trees yes)
	(treeshade no)
	(grass yes)
	=>
	(location "Savannah")
)
(defrule moutane-grassland (declare (salience 10))
	(altitude high)
	(grass yes)
	=>
	(location "Montane grassland")
)
(defrule tundra (declare (salience 10))
	(flat yes)
	(grass yes)
	(temperature low)
	=>
	(location "Tundra")
)
(defrule woodland (declare (salience 10))
	(trees yes)
	(treeshade yes)
	(temperature medium)
	=>
	(location "Moderate woodland")
)
(defrule grassland (declare (salience 10))
	(grass yes)
	(flat yes)
	(temperature medium)
	=>
	(location "Moderate grassland")
)
(defrule arctic-ocean (declare (salience 10))
	(underwater yes)
	(iceeverywhere yes)
	=>
	(location "Arctic ocean")
)
