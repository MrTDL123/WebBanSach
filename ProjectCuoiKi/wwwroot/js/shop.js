document.addEventListener("DOMContentLoaded", function () {
    const optionmenu = document.querySelector('.drop_down_menu')
    const selecIcon = document.querySelector('.drop_menu_content')
    const choose = document.querySelectorAll('.choosen_option')
    const chon_text = document.querySelector('.in_chon_text')

    selecIcon.addEventListener('click', () => optionmenu.classList.toggle('active'))
    choose.forEach(option => {
        option.addEventListener('click', () => {
            let selectedOption = option.querySelector('.choosen_option_text').innerText
            // choose_text.innerText = selectedOption;
            chon_text.innerText = selectedOption
        })
    })
})


document.addEventListener("DOMContentLoaded", function () {
    // Code Price range
    let rangeInput = document.querySelectorAll('.range_input input')
    let rangeText = document.querySelectorAll('.range_text div')
    let progress = document.querySelector('.progress')
    let priceMax = rangeInput[0].max // priceMax sẽ trỏ đến giá trị max bên trong mỗi thẻ input[0] đầu tiên
    // let priceMin = rangeInput[1].min // priceMin sẽ trỏ đến giá trị min bên trong thẻ input[1]

    // vì việc 2 giá trị min và max có thể chạy lung tung, min có thể lớn hơn max
    let priceGap = 10000 //Khoảng cách tối thiểu của min và max


    /* tiến hành chạy 1 vòng forEach để lấy từng input ra với mỗi input
        - Với mỗi input phải lắng nghe 1 sự kiện thay đổi giá trị thì lúc này nó sẽ chạy
        1 function với toàn bộ dữ liệu của sự kiện đó là event
        - Giá trị input có value tối thiểu là 50.000 và giá trị tối đa là 5.000.000 
        - Để đạt chiều dài lớn nhất là 100% thì giá trị của nó phải là 5.000.000 - 50.000
        - Giá trị input_min hiện tại có giá trị là 50.000
        - Giả sử giá trị min_input này có giá trị là 100.000 thì cái progress cũng phải bị thu nhỏ 
        theo input_min value bởi vì progress phản ánh điểm giao của 2 input
        => Xác định khoảng cách theo phần trăm của input_min và sau đó gán vào cho progess
        + Cách chuyển 100.000 ra phần trăm:
    
        value = (value/priceMax)*100 
    
        * Trong đó: 
        + priceMax là giá trị tối đa 
    
        100.000 = (100.000/5.000.000)*100 = 2% 
        vậy là cái minPrice này cách left 2% và ta sẽ lấy giá trị này gán cho progress
        để cái hiệu ứng progress sẽ đi theo minPrice
    
        - minVal: Giá trị range input đầu tiên tức là min_input
        + Để sử dụng giá trị minVal trong tính toán thì ta phải ép kiểu nó về Int
        - positionMin: Giá trị cách left của vị trí min khi chuyển về phần trăm
    
        - đối với vị trí của max_input khi tính theo khoảng cách left chuyển về phần trăm thì
        Giả sử giá trị max_input có giá trị là 3.000.000
        => Khoảng cách của nó so với left là: 3.000.000 = (3.000.000/5.000.000)*100
        => Tuy nhiên max_input mình sẽ căn chỉnh theo right chứ không phải left
        Khoảng cách của max_input so với right lúc này sẽ bằng 100 - khoảng cách so với left
        100 - (3.000.000/5.000.000)*100
    */

    // Hàm thêm dấu chấm phẩy phần nghìn
    function format_currency(value) {
        return value.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".");
    }

    rangeInput.forEach(input => {
        input.addEventListener('input', (event) => {
            let minVal = parseInt(rangeInput[0].value)
            let maxVal = parseInt(rangeInput[1].value)

            // kiểm tra khoảng cách hợp lệ của min và max
            if ((maxVal - minVal) < priceGap) {
                /* Nếu điều này ko hợp lệ thì kiểm tra xem input của thằng nào 
                    đang cố thay đổi giá trị sai 
                    với event.target: thằng đang được thay đổi giá trị
                */
                if (event.target.classname === 'range_min') {
                    // sửa lại giá trị cho thằng minVal chỉ có giá trị tối đa là
                    minVal = rangeInput[0].value = maxVal - priceGap
                }
                else {
                    maxVal = rangeInput[1].value = minVal + priceGap
                }
            }

            let positionMin = (minVal / priceMax) * 100
            progress.style.left = positionMin + "%"
            let positionMax = 100 - (maxVal / priceMax) * 100
            progress.style.right = positionMax + "%"

            // Hiển thị giá trị của min và max range
            // Thay đổi giá trị khoảng cách left và right của nó
            // rangeText[0].style.left = positionMin + "%"

            // Sử innerText để thay đổi nội dung cho từng đoạn text hiển thị min và max input
            rangeText[0].innerText = format_currency(minVal)
            rangeText[1].innerText = format_currency(maxVal)
        })
    })

})

