import React, { useState } from "react";
import "../../assets/css/trans.css";

const Transac = () => {
  const [selectedProducts, setSelectedProducts] = useState([]);

  const handleSelectProduct = (event) => {
    const productId = event.target.value;
    if (event.target.checked) {
      setSelectedProducts([...selectedProducts, productId]);
    } else {
      setSelectedProducts(selectedProducts.filter(id => id !== productId));
    }
  };

  const handlePrint = () => {
    const newWindow = window.open('', '', 'height=800,width=600');
    newWindow.document.write('<html><head><title>Print</title>');
    newWindow.document.write('<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" type="text/css" />');
    newWindow.document.write('<style>');
    newWindow.document.write('@media print { body { -webkit-print-color-adjust: exact; margin: 20px; font-size: 18px; }');
    newWindow.document.write('.printable-content { width: 100%; margin: auto; }');
    newWindow.document.write('.card-body { padding: 20px; border: 1px solid #dee2e6; border-radius: 4px; }');
    newWindow.document.write('.media-1 { margin-bottom: 20px; display: flex; align-items: center; }');
    newWindow.document.write('.media-body { padding: 10px; }');
    newWindow.document.write('img { max-width: 100%; height: auto; }');
    newWindow.document.write('.h5, .h5 a { font-size: 1.5rem; font-weight: 500; margin-bottom: 0.5rem; }');
    newWindow.document.write('.text-body { font-size: 1rem; color: #000; }');
    newWindow.document.write('.d-print-none { display: none; }'); // Hide print button
    newWindow.document.write('</style>');
    newWindow.document.write('</head><body class="printable-content">');
    
    selectedProducts.forEach(id => {
      const printContents = document.getElementById(id).innerHTML;
      newWindow.document.write(printContents);
      newWindow.document.write('<hr>'); // Add a separator between products
    });
    
    newWindow.document.write('</body></html>');

    newWindow.document.close();
    newWindow.focus(); // Necessary for IE >= 10
    newWindow.print();
  };
  const getCurrentDate = () => {
    const date = new Date();
    const options = { year: "numeric", month: "long", day: "numeric" };

    return date.toLocaleDateString("en-US", options);
  };
  const styles = {
    container: {
      display: "flex",
      alignItems: "center",
    },
    title: {
      fontWeight: "bold",
      color: "#2c2e33",
      marginRight: "0.5rem",
    },
    badge: {
      display: "flex",
      alignItems: "center",
      backgroundColor: "#e6f7e9",
      color: "#28a745",
      fontWeight: "bold",
      padding: "0.2rem 0.75rem",
      borderRadius: "0.25rem",
      marginLeft: "1rem",
    },
    indicator: {
      display: "inline-block",
      width: "0.75rem",
      height: "0.75rem",
      borderRadius: "50%",
      backgroundColor: "#28a745",
      marginRight: "0.5rem",
    },
    date: {
      marginLeft: "0.5rem",
    },
  };
  return (
    <main id="content" role="main" className="main">
      <div className="content container-fluid">
        <div className="page-header d-print-none">
          <div className="row align-items-center">
            <div className="col-sm mb-2 mb-sm-0">
              <nav aria-label="breadcrumb">
                <ol className="breadcrumb breadcrumb-no-gutter">
                  <li className="breadcrumb-item">
                    <a className="breadcrumb-link" href="ecommerce-orders.html">
                      Orders
                    </a>
                  </li>
                  <li className="breadcrumb-item active" aria-current="page">
                    Order details
                  </li>
                </ol>
              </nav>

              <div style={styles.container}>
                <h1 style={styles.title}>Order #32543</h1>
                <span style={styles.badge}>
                  <span style={styles.indicator}></span> PAID
                </span>
                <span style={styles.date}>
                  <i className="tio-date-range"></i> {getCurrentDate()}
                </span>
              </div>
            </div>
          </div>
        </div>

        <div className="container">
      <div className="row">
        <div className="col-lg mb-3 mb-lg-0">
          <div className="card mb-3 mb-lg-5">
            <div className="card-header">
              <h4 className="card-header-title">
                Order details
                <span className="badge badge-soft-dark rounded-circle ml-1">4</span>
              </h4>
            </div>

            <div className="card-body">
              <div id="product-1" className="media-1 mb-3">
              <input type="checkbox" value="product-1" onChange={handleSelectProduct} className="mr-2"/>
                <div className="avatar avatar-xl mr-3">
                  <img
                    className="img-fluid"
                    src="assets/img/400x400/img26.jpg"
                    alt="Topman shoe in green"
                  />
                </div>
                <div className="media-body">
                  <div className="row">
                    <div className="col-md-8 mb-3 mb-md-0">
                      <a className="h5 d-block" href="ecommerce-product-details.html">Topman shoe in green</a>
                      <div className="font-size-sm text-body">
                        <span>Gender: </span>
                        <span className="font-weight-bold">Women</span>
                      </div>
                      <div className="font-size-sm text-body">
                        <span>Color: </span>
                        <span className="font-weight-bold">Green</span>
                      </div>
                      <div className="font-size-sm text-body">
                        <span>Size: </span>
                        <span className="font-weight-bold">UK 7</span>
                      </div>
                    </div>
                    <div className="col-md-4 align-self-center text-right">
                      <h5 className="mb-0">$42.00</h5>
                    </div>
                  </div>
                </div>
              </div>

              <hr />

              <div id="product-2" className="media-1 mb-3">
              <input type="checkbox" value="product-2" onChange={handleSelectProduct} className="mr-2"/>
                <div className="avatar avatar-xl mr-3">
                  <img
                    className="img-fluid"
                    src="assets/img/400x400/img22.jpg"
                    alt="Office Notebook"
                  />
                </div>
                <div className="media-body">
                  <div className="row">
                    <div className="col-md-8 mb-3 mb-md-0">
                      <a className="h5 d-block" href="ecommerce-product-details.html">Office Notebook</a>
                      <div className="font-size-sm text-body">
                        <span>Color: </span>
                        <span className="font-weight-bold">Gray</span>
                      </div>
                    </div>
                    <div className="col-md-4 align-self-center text-right">
                      <h5 className="mb-0">$9.00</h5>
                    </div>
                  </div>
                </div>
              </div>

              <hr />

              <div id="product-3" className="media-1 mb-3">
              <input type="checkbox" value="product-3" onChange={handleSelectProduct} className="mr-2"/>
                <div className="avatar avatar-xl mr-3">
                  <img
                    className="img-fluid"
                    src="assets/img/400x400/img15.jpg"
                    alt="RayBan sunglasses"
                  />
                </div>
                <div className="media-body">
                  <div className="row">
                    <div className="col-md-8 mb-3 mb-md-0">
                      <a className="h5 d-block" href="ecommerce-product-details.html">RayBan sunglasses</a>
                      <div className="font-size-sm text-body">
                        <span>Gender: </span>
                        <span className="font-weight-bold">Unisex</span>
                      </div>
                      <div className="font-size-sm text-body">
                        <span>Color: </span>
                        <span className="font-weight-bold">Black</span>
                      </div>
                      <div className="font-size-sm text-body">
                        <span>Size: </span>
                        <span className="font-weight-bold">One size</span>
                      </div>
                    </div>
                    <div className="col-md-4 align-self-center text-right">
                      <h5 className="mb-0">$14.00</h5>
                    </div>
                  </div>
                </div>
              </div>
              <button className="btn btn-primary mt-2" onClick={handlePrint}>
                Print Selected Products
              </button>
              <hr />
            </div>
          </div>
        </div>
      </div>
    </div>
      </div>
    </main>
  );
};

export default Transac;
