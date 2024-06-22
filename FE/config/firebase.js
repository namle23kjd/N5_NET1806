// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries
import { getStorage } from "firebase/storage";
import { GoogleAuthProvider } from "firebase/auth";
import { getAuth } from "firebase/auth";

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: "AIzaSyBOu1-3hf6KqyE2m0uwYR9UdJVT4oXss0g",
  authDomain: "petspa-0808.firebaseapp.com",
  projectId: "petspa-0808",
  storageBucket: "petspa-0808.appspot.com",
  messagingSenderId: "859604457940",
  appId: "1:859604457940:web:37f93ac30802b104f85e41",
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const storage = getStorage(app);
const googleprovider = new GoogleAuthProvider();
const auth = getAuth();
export { auth, storage, googleprovider };
