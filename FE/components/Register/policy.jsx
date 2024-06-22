

const PrivacyPolicyTerms = () => {
    const getCurrentDate = () => {
        const date = new Date();
        const options = { year: 'numeric', month: 'long', day: 'numeric' };
        return date.toLocaleDateString('en-US', options);
      };
  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-semibold mb-4">Privacy Policy & Terms of Service</h1>

      <h2 className="text-xl font-semibold mb-2">1. Introduction</h2>
      <p>
        Welcome to Pets' Spa Management! This document outlines our policies regarding the collection,
        use, and disclosure of personal information when you use our service. By using our service, you agree
        to the collection and use of information in accordance with this policy.
      </p>

      <h2 className="text-xl font-semibold mb-2">2. Information Collection and Use</h2>
      <p>
        We collect several types of information for various purposes to provide and improve our service to you.
        Types of data collected may include personal data such as name, email address, phone number, and pet
        details necessary for providing spa services.
      </p>

      <h2 className="text-xl font-semibold mb-2">3. Use of Data</h2>
      <p>
        The information we collect is used to operate and improve our services. This may include communicating
        with you, processing payments, and providing personalized services.
      </p>

      <h2 className="text-xl font-semibold mb-2">4. Data Security</h2>
      <p>
        We value your trust in providing us with your personal information. We use industry-standard protocols
        to protect your data from unauthorized access or disclosure.
      </p>

      <h2 className="text-xl font-semibold mb-2">5. Sharing of Data</h2>
      <p>
        We may share your information with trusted third parties to provide services on our behalf or to
        comply with legal obligations.
      </p>

      <h2 className="text-xl font-semibold mb-2">6. Changes to This Privacy Policy</h2>
      <p>
        We may update our Privacy Policy from time to time. You are advised to review this policy periodically
        for any changes. Changes to this Privacy Policy are effective when they are posted on this page.
      </p>

      <h2 className="text-xl font-semibold mb-2">7. Contact Us</h2>
      <p>
        If you have any questions about this Privacy Policy, please contact us:
        <br />
        Email: petspaswp391@gmail.com
        <br />
        Phone: +84 (012) 345-6789
      </p>

      <h2 className="text-xl font-semibold mb-2">Terms of Service</h2>
      <p>
        By accessing or using the Pets' Spa Management website or mobile application (the "Service"), you agree to
        be bound by these Terms of Service and our Privacy Policy. Please read these Terms carefully before accessing
        or using our Service.
      </p>

      <h3 className="text-lg font-semibold mb-2">1. Use of the Service</h3>
      <p>
        You may use our Service only as permitted by law and these Terms. Your use of the Service is conditioned
        upon your acceptance of and compliance with these Terms.
      </p>

      <h3 className="text-lg font-semibold mb-2">2. User Accounts</h3>
      <p>
        To access certain features of the Service, you may be required to create a user account. You are responsible
        for maintaining the confidentiality of your account and password and for restricting access to your account.
      </p>

      <h3 className="text-lg font-semibold mb-2">3. Modifications to the Service</h3>
      <p>
        We reserve the right to modify or discontinue, temporarily or permanently, the Service (or any part thereof)
        with or without notice.
      </p>

      <h3 className="text-lg font-semibold mb-2">4. Governing Law</h3>
      <p>
        These Terms shall be governed and construed in accordance with the laws of [Your Country], without regard to
        its conflict of law provisions.
      </p>

      <p className="mt-8">
        Last updated: {getCurrentDate()}
      </p>
    </div>
  );
};

export default PrivacyPolicyTerms;