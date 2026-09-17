package main

import (
	"crypto/subtle"
	"fmt"
	"log"
	"net/http"
)

const (
	authUser = "admin"
	authPass = "tajne123"
	realm    = "StrefaPrywatna"
)

func basicAuthenticationHandler(w http.ResponseWriter, r *http.Request) {
	user, pass, ok := r.BasicAuth()

	userMatch := subtle.ConstantTimeCompare([]byte(user), []byte(authUser)) == 1
	passMatch := subtle.ConstantTimeCompare([]byte(pass), []byte(authPass)) == 1

	if !ok || !userMatch || !passMatch {
		w.Header().Set("WWW-Authenticate", fmt.Sprintf(`Basic realm="%s", charset="UTF-8"`, realm))
		http.Error(w, "Wymagana autoryzacja", http.StatusUnauthorized)
		return
	}
}

func main() {
	http.HandleFunc("/", basicAuthenticationHandler)
	fmt.Println("Listening on http://+:8080 ...")
	if err := http.ListenAndServe(":8080", nil); err != nil {
		log.Fatal(err)
	}
}
